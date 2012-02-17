<h1>Neighborparrot .NET client</h1>


<h2>How to send messages to the NeighborParrot Service</h2>

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
            Parrot.Configure(new SortedList(){{"api_id","your_api_id"},{"api_key","yout_api_key"}});
            Parrot _parrot = new Parrot();
            _parrot.Send("test", "hellow world from .net");
        }
    }
}
</code></pre>