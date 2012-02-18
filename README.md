<h1>Neighborparrot .NET client</h1>


<h3>How to send messages to the NeighborParrot Service</h3>

<pre><code>
using System;
using System.Collections;
using ParrotDotNet;

namespace WebApplication
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {            
			// Configure the Parrot
            Parrot.Configure(new SortedList(){{"api_id","your_api_id"},{"api_key","your_api_key"}});

            Parrot _parrot = new Parrot();
			// Send a message to the channel test
            string message_id = _parrot.Send("test", "hello world from .net");
        }
    }
}
</code></pre>