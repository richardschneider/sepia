using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia
{
    class MultilingualTextExample
    {
        void Snippets()
        {
            #region Hello World
            var greetings = new MultilingualText
            {
                new Text("en", "hello world"),
                new Text("en-AU", "g'day mate"),
                new Text("en-NZ", "cheers"),
                new Text("zh-Hant", "你好")
            };

            // Produces: cheers
            Console.WriteLine(greetings["en-NZ"].Value);

            // Produces: hello world
            Console.WriteLine(greetings["en-US"].Value);

            // Produces: 你好
            Console.WriteLine(greetings["zh-Hant"].Value);
            #endregion

        }
    }
}
