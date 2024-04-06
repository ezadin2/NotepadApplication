// FileOperation.cs
using System;

namespace NotepadCore
{
    public class FileOperation
    {
        private string filename = string.Empty; // Initialize the field to avoid CS8618 warning
        private bool isFileSaved;

        public string Filename { get => filename; set => filename = value; }
        public bool IsFileSaved { get => isFileSaved; set => isFileSaved = value; }

        public void InitializeNewFile()
        {
            this.Filename = "Untitled.txt";
            this.IsFileSaved = true;
        }

        public void SaveFile(string fileName, string[] lines)
        {
           
        }
    }
}
