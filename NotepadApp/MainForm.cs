using NotepadApp.Functionality;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NotepadApp
{
    public partial class MainForm : Form
    {
        FileOperation fileOperation;
        EditOperation editOperation;
        Timer timer;
        FormFind formFind;
        FormAbout about;
        public EditOperation EditOperation
        {
            get
            {
                return editOperation;
            }

            set
            {
                editOperation = value;
            }
        }

        public MainForm()
        {
            InitializeComponent();
            fileOperation = new FileOperation();
            editOperation = new EditOperation();
            formFind = new FormFind(this);
            formFind.Editor = txtArea;
            fileOperation.InitializeNewFile();
            this.Text = fileOperation.Filename;
            timer = new Timer();
            timer.Tick += Mytimer_Tick;
            timer.Interval = 500;
            txtArea.HideSelection = false;
        }

        private void Mytimer_Tick(object sender, EventArgs e)
        {
            timer.Stop();

            UpdateView();
        }

        private void UpdateView()
        {
            this.Text = !fileOperation.IsFileSaved ? fileOperation.Filename + "*" : fileOperation.Filename;

            findNext.Enabled = findNext.Enabled;

        }

        private void newFileMenu_Click(object sender, EventArgs e)
        {
            if (fileOperation.IsFileSaved)
            {
               
                txtArea.Text = "";
                fileOperation.InitializeNewFile();
                UpdateView();
            }
            else
            {
                DialogResult result = MessageBox.Show("Do you need save changes to " + fileOperation.Filename, "Notepad", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (fileOperation.Filename.Contains("Untitled"))
                    {
                        SaveFileDialog newFileSave = new SaveFileDialog();
                        newFileSave.Filter = "Text(*.txt)|*.txt";
                        if (newFileSave.ShowDialog() == DialogResult.OK)
                        {
                            fileOperation.SaveFile(newFileSave.FileName, txtArea.Lines);
                            UpdateView();
                        }
                    }
                    else
                    {
                        fileOperation.SaveFile(fileOperation.FileLocation, txtArea.Lines);
                        UpdateView();
                    }

                }
                else if (result == DialogResult.No)
                {
                    txtArea.Text = "";
                    fileOperation.InitializeNewFile();
                    UpdateView();
                }


            }

        }

        private void txtArea_TextChanged(object sender, EventArgs e)
        {
            fileOperation.IsFileSaved = false;
            if (editOperation.TxtAreaTextChangeRequired)
            {
                timer.Start();
            }
            else
            {
                editOperation.TxtAreaTextChangeRequired = false;
            }
            UpdateView();
        }

        private void openMenu_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Text(*.txt)|*.txt";
            openFile.InitialDirectory = "D:";
            openFile.Title = "Open File";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                txtArea.TextChanged -= txtArea_TextChanged;
                txtArea.Text = fileOperation.OpenFile(openFile.FileName);
                txtArea.TextChanged += txtArea_TextChanged;
                UpdateView();
            }
        }

        private void saveFileMenu_Click(object sender, EventArgs e)
        {
            if (!fileOperation.IsFileSaved)
            {
                if (!this.Text.Contains("Untitled.txt"))
                {
                    fileOperation.SaveFile(fileOperation.FileLocation, txtArea.Lines);
                    UpdateView();
                }
                else
                {
                    SaveFile();
                }
            }
        }
        private void SaveFile()
        {
            SaveFileDialog fileSave = new SaveFileDialog();
            fileSave.Filter = "Text(*.txt)|*.txt";
            if (fileSave.ShowDialog() == DialogResult.OK)
            {
                fileOperation.SaveFile(fileSave.FileName, txtArea.Lines);
                UpdateView();
            }
        }

        private void saveasFileMenu_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void find_Click(object sender, EventArgs e)
        {
            if (formFind == null)
            {
                formFind = new FormFind(this);
                formFind.Editor = txtArea;
            }
            formFind.Show();
        }

        private void findNext_Click(object sender, EventArgs e)
        {
            formFind.UpdateSearchQuery();
            if (formFind.Qry.SearchString.Length == 0)
            {
                formFind.Show();
            }
            else
            {
                FindNextResult result = editOperation.FindNext(formFind.Qry);
                if (result.SearchStatus)
                    txtArea.Select(result.SelectionStart, formFind.Qry.SearchString.Length);
            }
        }

        private void pageSetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PageSetupDialog pageSetupDialog = new PageSetupDialog();
            pageSetupDialog.PageSettings = new System.Drawing.Printing.PageSettings();

            if (pageSetupDialog.ShowDialog() == DialogResult.OK)
            {

                System.Drawing.Printing.PageSettings pageSettings = pageSetupDialog.PageSettings;
                float leftMargin = pageSettings.Margins.Left;
                float rightMargin = pageSettings.Margins.Right;
                float topMargin = pageSettings.Margins.Top;
                float bottomMargin = pageSettings.Margins.Bottom;
            }
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            PrintDocument printDocument = new PrintDocument();

            printDialog.Document = printDocument;

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDocument.PrintPage += new PrintPageEventHandler(printDocument_PrintPage);
                printDocument.Print();
            }
        }

        private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
            if (txtArea.SelectedText != "")
            {
               
                Clipboard.SetText(txtArea.SelectedText);

              
                txtArea.SelectedText = "";
            }
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
            if (Clipboard.ContainsText())
            {
                
                txtArea.SelectedText = Clipboard.GetText();
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if (txtArea.SelectedText != "")
            {
               
                Clipboard.SetText(txtArea.SelectedText);
            }
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (about == null)
            {
                about = new FormAbout();
               about.Editor = txtArea;
                about.editOpertion = editOperation;
            }
            about.Show();
        }

        private void fontMenu_Click(object sender, EventArgs e)
        {
            FontDialog fontDialog1 = new FontDialog();
            fontDialog1.ShowColor = true;
            fontDialog1.Font = txtArea.Font;
            fontDialog1.Color = txtArea.ForeColor;

            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                txtArea.Font = fontDialog1.Font;
                txtArea.ForeColor = fontDialog1.Color;
            }
        }

        private void wordWrapMenu_Click(object sender, EventArgs e)
        {
            txtArea.WordWrap = !txtArea.WordWrap;
        }

        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtArea.Font = new Font(txtArea.Font.FontFamily, txtArea.Font.Size + 1);
        }

       

        private void zoomInToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            txtArea.Font = new Font(txtArea.Font.FontFamily, txtArea.Font.Size + 1);
        }

        private void zoomOutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            txtArea.Font = new Font(txtArea.Font.FontFamily, txtArea.Font.Size - 1);
        }

        private void pToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
