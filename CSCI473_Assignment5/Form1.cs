using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Assignment5
{
    public partial class Form1 : Form
    {
        public string openPuzzlesPath = (@"..\..\directory.txt");
        public string currentPuzzlePath = (@"..\..\puzzles\");
        public List<string> puzzles = new List<string>();
        public char[] currentPuzzle = new char[81];
        public string difficulty = "easy";
        IEnumerable<RichTextBox> allRichTextBoxes;
        RichTextBox[] sortedRichTextBoxes;

        public Form1()
        {
            InitializeComponent();
            GetPuzzles();
            allRichTextBoxes = panel10.Controls.OfType<RichTextBox>();
            sortedRichTextBoxes = allRichTextBoxes
                 .OrderBy(i => i.Name)
                 .ToArray();
        }
    

        private void CharacterCheck(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                e.Handled = true;
        }

        private void HighlightFocus(object sender, EventArgs e)
        {
            RichTextBox richTextBox = (sender as RichTextBox);
            richTextBox.BackColor = Color.SkyBlue;
        }

        private void LeaveFocus(object sender, EventArgs e)
        {
            RichTextBox richTextBox = (sender as RichTextBox);
            richTextBox.BackColor = Color.White;
        }

        private void GetPuzzles()
        {
            StreamReader stream = new StreamReader(openPuzzlesPath);
            string file;
            while ((file = stream.ReadLine()) != null)
                puzzles.Add(file);
            stream.Close();
        }

        private void NewPuzzle(string difficulty)
        {
            currentPuzzlePath = (@"..\..\puzzles\");
            var random = new Random();
            List<string> difficultyPuzzles = puzzles.FindAll(o => o.Contains(difficulty));
            int index = random.Next(difficultyPuzzles.Count);
            currentPuzzlePath += difficultyPuzzles[index];
            BuildNewPuzzle();
        }

        private void BuildNewPuzzle()
        {
            StreamReader stream = new StreamReader(currentPuzzlePath);
            string line;
            string puzzle = null;
            while ((line = stream.ReadLine()) != null)
                puzzle += line;
            char[] newPuzzle = puzzle.Substring(0, 81).ToCharArray();
            currentPuzzle = puzzle.Substring(81, 81).ToCharArray();
            if (puzzle.Length > 162)
            {
                newPuzzle = puzzle.Substring(162, 81).ToCharArray();
            }
            stream.Close();
            PopulateBoard(newPuzzle);
        }

        private void NewPuzzleButton_Click(object sender, EventArgs e)
        {
            difficultyContextMenuStrip.Show(newPuzzleButton, new Point(0, newPuzzleButton.Height));
        }

        private void DifficultyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (sender as ToolStripMenuItem);
            difficulty = menuItem.Text.ToLower();
            NewPuzzle(difficulty);
        }

        private void PopulateBoard(char[] newPuzzle)
        {
            int index = 0;
            foreach (var richTextBox in sortedRichTextBoxes)
            {
                richTextBox.ReadOnly = false;
                if (newPuzzle[index] == '0')
                    richTextBox.Text = "";
                else
                {
                    richTextBox.Text = newPuzzle[index].ToString();
                    (richTextBox as RichTextBox).ReadOnly = true;
                }
                index++;
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            File.Copy(currentPuzzlePath, @"..\..\puzzles\saved\saved.txt", true);
            int index = 0;
            int newlineCounter = 1;
            using (StreamWriter sw = File.AppendText(@"..\..\puzzles\saved\saved.txt"))
            {
                sw.WriteLine();
                sw.WriteLine();

                foreach (var richTextBox in sortedRichTextBoxes)
                {
                    if (richTextBox.Text == "")
                        sw.Write('0');
                    else
                    {
                        sw.Write(richTextBox.Text);
                    }
                    newlineCounter++;
                    index++;
                    if (newlineCounter > 9)
                    {
                        sw.WriteLine();
                        newlineCounter = 1;
                    }
                }
            }
        }

        private void ResumedSave_Click(object sender, EventArgs e)
        {
            currentPuzzlePath = (@"..\..\puzzles\saved\saved.txt");
            BuildNewPuzzle();
        }
    }
}