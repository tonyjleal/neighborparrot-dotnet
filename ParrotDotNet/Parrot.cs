using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Net;
using System.Web.Script.Serialization;

namespace ParrotDotNet
{
    public class Parrot
    {
        private static string server_host = "https://neighborparrot.net";
        private static SortedList options;

        /// <summary>
        /// Configure default options, like api_id and api_key
        /// </summary>
        /// <param name="options">Options to configure. Valid options are: api_id,api_key</param>
        public static void Configure(SortedList opt)
        {
            if (String.IsNullOrEmpty(Convert.ToString(opt["api_id"])))
                throw new ArgumentNullException("api_id");
            if (String.IsNullOrEmpty(Convert.ToString(opt["api_key"])))
                throw new ArgumentNullException("api_key");

            options = opt;
        }

        /// <summary>
        /// Send a message to the given channel
        /// </summary>
        /// <param name="channel">channel name</param>
        /// <param name="message">message to send</param>
        /// <returns>Response from the server with error or message id if success</returns>
        public void Send(String channel, String message)
        {
            string signed_data = sign_send_request(channel, message);
            string url = server_host + "/send?" + signed_data;

            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                string json = new JavaScriptSerializer().Serialize(message);
                client.UploadString(url, json);
            }
        }
        /// <summary>
        /// Return signed data for send request
        /// </summary>
        /// <param name="channel">desired channel</param>
        /// <param name="message">message to send. Can be some kind of string serialization</param>
        /// <returns>signed data with auth fields</returns>
        private string sign_send_request(String channel, String message)
        {
            SortedList data = new SortedList() {{"channel",channel},
                                                {"data",message}};
            string type = "POST\n/send\n";
            return sign_request(type, data);
        }


        /// <summary>
        /// Add the required auth fields and sing the request.
        /// </summary>
        /// <param name="type"> type of request ([GET|POST]\n[/open|/ws] </param>
        /// <param name="data"> data to sign </param>
        /// <returns>signed data with auth fields.</returns>
        private String sign_request(String type, SortedList data)
        {
            int time = (int)((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);

            data.Add("auth_key", options["api_id"]);
            data.Add("auth_timestamp", time.ToString());
            data.Add("auth_version", "1.0");

            string str_to_sign = type + array_to_query(data);

            var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(Convert.ToString(options["api_key"])));
            var hash = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(Convert.ToString(str_to_sign)));
            return array_to_query(data) + "&auth_signature=" + BytesToHex(hash);
        }

        private static string BytesToHex(IEnumerable<byte> byteArray)
        {
            return String.Concat(byteArray.Select(bytes => bytes.ToString("x2")).ToArray());
        }


        /* Generate a valid url query from an array
         * Used when sign the requests.
        */
        private string array_to_query(SortedList parameters)
        {
            string array_map = String.Empty;
            int i = 0;
            foreach (DictionaryEntry p in parameters)
            {
                array_map += String.Format("{0}={1}", p.Key, p.Value);
                if (++i != parameters.Count)
                    array_map += "&";
            }
            return array_map;
        }

    }
}
