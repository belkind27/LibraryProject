using LibraryLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using System.IO;

namespace LibraryEx
{//a static class that helps the ui to be more efficient and helps pages communicate
    static class UiMeneger
    {
       static public Person CurrentUser { get; set; }
    }

}
