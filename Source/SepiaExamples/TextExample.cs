using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia
{
    class TextExample
    {
        void Snippets()
        {
            #region Hello World
            var greetings = new Text[]
             {
                new Text("en", "hello world"),
                new Text("en-AU", "g'day mate"),
                new Text("en-NZ", "cheers"),
            };

            // Produces: cheers
            Console.WriteLine(greetings.WrittenIn("en-NZ").Value);

            // Produces: hello world
            Console.WriteLine(greetings.WrittenIn("en-US").Value);
            #endregion

        }
    }
}
