using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Compositeパターン [C#][C++] - Qiita
//https://qiita.com/WestRiver/items/11c48ec3929322e296a7
//より
namespace _20220921
{
    internal class Class3
    {
        public Class3()
        {
            MyDirectory root = new("root");
            MyDirectory dir1 = new("dir1");
            MyDirectory dir2 = new("dir2");
            MyDirectory dir3 = new("dir3");
            MyFile file1 = new("file1");
            MyFile file2 = new("file2");
            MyFile file3 = new("file3");
            MyFile file4 = new("file4");

            root.AddEntry(dir1);
            root.AddEntry(dir2);
            dir2.AddEntry(dir3);

            root.AddEntry(file1);
            root.AddEntry(file2);
            dir2.AddEntry(file3);
            dir3.AddEntry(file4);

            root.Output(0);
            
        }

    }

    public interface IEntry { void Output(int someDepth); }
    public class MyDirectory : IEntry
    {
        private string? Name = null;
        private List<IEntry> Entries = new();
        public MyDirectory(string name) { Name = name; }
        public void AddEntry(IEntry entry) { Entries.Add(entry); }
        public void Output(int someDepth)
        {
            for (int i = 0; i < someDepth; i++)
            {
                Debug.Write("    ");
            }
            Debug.WriteLine($"{GetType()}, {Name}");
            foreach (var item in Entries)
            {
                item.Output(someDepth + 1);
            }
        }
    }
    public class MyFile : IEntry
    {
        private string? Name = null;
        public MyFile(string? name)
        {
            Name = name;
        }

        public void Output(int someDepth)
        {
            for (int i = 0; i < someDepth; i++)
            {
                Debug.Write("    ");
            }
            Debug.WriteLine($"{GetType()}, {Name}");
        }
    }
}
