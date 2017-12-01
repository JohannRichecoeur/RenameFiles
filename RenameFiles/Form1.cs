using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RenameFiles
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            this.InitializeComponent();
            this.AllowDrop = true;
            this.DragEnter += this.Form1DragEnter;
            this.dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dataGridView.Columns.Add(new DataGridViewColumn(new DataGridViewTextBoxCell()));
            this.dataGridView.Columns.Add(new DataGridViewColumn(new DataGridViewTextBoxCell()));

            this.ProcessingLabel.Text = "";
        }

        private void FolderButtonClick(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                var folder = this.folderBrowserDialog.SelectedPath;
                var folderName = folder.Split('\\').Last();
                this.FillFiles(Directory.GetFiles(folder), folderName);
            }
        }

        private void RenameButtonClick(object sender, EventArgs e)
        {
            // First use basic file names to avoid conflicts
            var count = 0;
            foreach (var g in from DataGridViewRow g in this.dataGridView.Rows where g.Cells[0].Value != null && g.Cells[1].Value != null select g)
            {
                var fileName = (string)g.Cells[0].Value;
                File.Move(fileName, fileName + "__" + count);
                count++;
            }

            // Then update to the final name
            count = 0;
            foreach (var g in from DataGridViewRow g in this.dataGridView.Rows where g.Cells[0].Value != null && g.Cells[1].Value != null select g)
            {
                ProcessingLabel.Text = count + 1 + " sur " + (this.dataGridView.Rows.Count - 1);
                ProcessingLabel.Refresh();

                File.Move((string)g.Cells[0].Value + "__" + count, (string)g.Cells[1].Value);
                count++;
            }
        }

        private void ClearButtonClick(object sender, EventArgs e)
        {
            this.dataGridView.Rows.Clear();
        }

        private void Form1DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void Form1DragDrop(object sender, DragEventArgs e)
        {
            var folders = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (var folder in folders)
            {
                if (!folder.ToUpper().EndsWith(".JPG"))
                {
                    this.FillFiles(Directory.GetFiles(folder), folder.Split('\\').Last());
                }
                else
                {
                    this.label1.Text = "Add a folder only, no files alone";
                    break;
                }
            }

        }

        private void FillFiles(string[] filePaths, string folderName)
        {
            this.label1.Text = folderName;

            var counter = 1;
            foreach (var filePath in filePaths)
            {
                ProcessingLabel.Text = counter + " sur " + filePaths.Count();
                ProcessingLabel.Refresh();

                string fileName = filePath;

                string prefix = folderName.Split('-').First() + "-";
                string file = fileName.Split('\\').Last();
                string path = fileName.Replace(file, "");

                string extension = "." + file.Split('.').Last();

                string zeroToAdd = "";

                if (filePaths.Count() > 100)
                {
                    if (counter < 10)
                    {
                        zeroToAdd = "00";
                    }
                    else if (counter < 100)
                    {
                        zeroToAdd = "0";
                    }
                    else
                    {
                        zeroToAdd = "";
                    }
                }
                else if (filePaths.Count() >= 10)
                {
                    zeroToAdd = counter < 10 ? "0" : "";
                }
                else if (filePaths.Count() < 11)
                {
                    zeroToAdd = "0";
                }

                file = prefix + zeroToAdd + counter;

                this.dataGridView.Rows.Add(fileName, path + file + extension);
                counter++;
            }
        }
    }
}