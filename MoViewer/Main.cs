using System;
using System.Drawing;
using System.Windows.Forms;

namespace MoViewer
{
    public partial class Main : Form
    {

        BindingSource Source = new BindingSource() { DataSource = new Msg[0] };

        public Main()
        {
            Font = SystemFonts.MessageBoxFont;
            InitializeComponent();
            grid.AutoGenerateColumns = false;
            splitContainer2.SplitterDistance = splitContainer2.Height / 2;

            grid.DataSource = Source;
            textBox1.DataBindings.Add("Text", Source, "Original");
            textBox2.DataBindings.Add("Text", Source, "Translation");
        }

        void LoadMo(string path)
        {
            try
            {
                var msgs = Mo.Parse(path);
                Source.DataSource = msgs;
                Text = "Mo Viewer - " + System.IO.Path.GetFileName(path);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void openItem_Click(object sender, EventArgs e)
        {
            if(openFile.ShowDialog() == DialogResult.OK)
            {
                LoadMo(openFile.FileName);
            }
        }

        private void Main_DragDrop(object sender, DragEventArgs e)
            => LoadMo((e.Data.GetData(DataFormats.FileDrop) as string[])[0]);

        private void Main_DragEnter(object sender, DragEventArgs e)
            => e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.All : DragDropEffects.None;
    }
}
