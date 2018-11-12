﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Assignment5
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);
        // variables
        public short hours, minutes, seconds = 0;
        public string difficulty = "easy";
        IEnumerable<RichTextBox> allRichTextBoxes;
        RichTextBox[] sortedRichTextBoxes;
        Color currentBackColor = Color.White;

        // directories and file paths
        public string openPuzzlesPath = (@"..\..\directory.txt");
        public string currentPuzzlePath = (@"..\..\puzzles\");
        public string savedPuzzlePath = null;
        public string openedPuzzle = null;
        // puzzle arrays
        public List<string> puzzles = new List<string>();           // list of puzzles
        public char[] currentPuzzleSolution = new char[81];         // solution of current puzzle
        public char[] savedPuzzle = new char[81];                   // copy of puzzle that is saved

        public Form1()
        {
            InitializeComponent();
            GetPuzzles();
            allRichTextBoxes = panel10.Controls.OfType<RichTextBox>();
            sortedRichTextBoxes = allRichTextBoxes
                 .OrderBy(i => i.Name)
                 .ToArray();
            savedPuzzle = null;
            NewPuzzle(difficulty);
        }
    

        private void CharacterCheck(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && (!char.IsDigit(e.KeyChar) || (e.KeyChar == '0')) && (e.KeyChar != '.'))
                e.Handled = true;
        }

        private void HighlightFocus(object sender, EventArgs e)
        {
            RichTextBox richTextBox = (sender as RichTextBox);
            currentBackColor = richTextBox.BackColor;
            richTextBox.BackColor = Color.SkyBlue;
        }

        private void LeaveFocus(object sender, EventArgs e)
        {
            RichTextBox richTextBox = (sender as RichTextBox);
            richTextBox.BackColor = currentBackColor;
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
            openedPuzzle = difficultyPuzzles[index];
            BuildNewPuzzle();
        }

        private void BuildNewPuzzle()
        {
            savedPuzzle = null;
            StreamReader stream = new StreamReader(currentPuzzlePath);
            string line;
            string puzzle = null;
            while ((line = stream.ReadLine()) != null)
                puzzle += line;
            char[] newPuzzle = puzzle.Substring(0, 81).ToCharArray();
            currentPuzzleSolution = puzzle.Substring(81, 81).ToCharArray();
            if (puzzle.Length > 162)
            {
                savedPuzzle = puzzle.Substring(162, 81).ToCharArray();
                SetTimer(puzzle.Substring(243));
                openedPuzzle = puzzle.Substring(249);
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
            char[] currentPuzzle = newPuzzle;
            if (savedPuzzle != null)
            {
                currentPuzzle = savedPuzzle;
            }
            int index = 0;
            foreach (var richTextBox in sortedRichTextBoxes)
            {
                richTextBox.ReadOnly = false;
                if (currentPuzzle[index] == '0')
                {
                    richTextBox.Text = "";
                    richTextBox.BackColor = Color.White;
                }
                else
                {
                    if (newPuzzle[index] != '0')
                    {
                        (richTextBox as RichTextBox).ReadOnly = true;
                    }
                    richTextBox.BackColor = Color.FromArgb(233, 233, 233);
                    richTextBox.Text = currentPuzzle[index].ToString();
                }
                index++;
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            savedPuzzlePath = (@"..\..\puzzles\saved\saved.txt");
            if (currentPuzzlePath == savedPuzzlePath)
            {
                File.Copy(@"..\..\puzzles\" + openedPuzzle, savedPuzzlePath, true);
            }
            else
                File.Copy(currentPuzzlePath, savedPuzzlePath, true);
            int index = 0;
            int newlineCounter = 1;
            using (StreamWriter sw = File.AppendText(savedPuzzlePath))
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
                sw.WriteLine(String.Format("{0}{1}{2}", hours.ToString("00"), minutes.ToString("00"), seconds.ToString("00")));
                sw.WriteLine(openedPuzzle);
            }
        }

        private void ResumedSave_Click(object sender, EventArgs e)
        {
            currentPuzzlePath = (@"..\..\puzzles\saved\saved.txt");
            BuildNewPuzzle();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        private void Pause_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            IncrementSeconds();
            UpdateTimer();
        }

        private void IncrementSeconds()
        {
            if (seconds == 59)
            {
                seconds = 0;
                IncrementMinutes();
            }
            else
            {
                seconds++;
            }
        }

        private void IncrementMinutes()
        {
            if (minutes == 59)
            {
                minutes = 0;
                IncrementHours();
            }
            else
            {
                minutes++;
            }
        }

        private void IncrementHours()
        {
            hours++;
        }

        private void StartTimer(object sender, KeyEventArgs e)
        {
            // here
            if ((sender as RichTextBox).ReadOnly == false)
                timer.Enabled = true;
        }

        private void Reset_Click(object sender, EventArgs e)
        {
            Pause_Click(sender, e);

            hours = 0;
            minutes = 0;
            seconds = 0;

            UpdateTimer();
        }

        private void CheatButton_Click(object sender, EventArgs e)
        {
            Random randomIndex = new Random();
            int randomIndexValue = randomIndex.Next(1, 81);
            while (sortedRichTextBoxes[randomIndexValue].Text!="")
            {
                randomIndexValue = randomIndex.Next(1, 81);
            }
            sortedRichTextBoxes[randomIndexValue].Text = currentPuzzleSolution[randomIndexValue].ToString();
            sortedRichTextBoxes[randomIndexValue].ForeColor = Color.Salmon;
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {

            if (IsComplete() == false)
            {
                MessageBox.Show("You made a mistake.");
            }
            else if (IsComplete() == true)
            {
                MessageBox.Show("You finished the puzzle with no mistakes!");
            }
            
        }

        private bool IsComplete()
        {
            bool win = false;
            for (int i = 0; i < 81; i++)
            {
                if (sortedRichTextBoxes[i].Text != currentPuzzleSolution[i].ToString())
                {
                    win = false;
                    break;
                }
                else
                {
                    win = true;
                }
            }
            return win;
        }

        private void RemoveCaret(object sender, MouseEventArgs e)
        {
            HideCaret((sender as RichTextBox).Handle);
        }

        private void RemoveCaret(object sender, EventArgs e)
        {
            HideCaret((sender as RichTextBox).Handle);
        }

        private void UpdateTimer()
        {
            timerText.Text = String.Format("{0}:{1}:{2}", hours.ToString("00"), minutes.ToString("00"), seconds.ToString("00"));
        }

        private void SetTimer(string savedTimer)
        {
            hours = Int16.Parse(savedTimer.Substring(0, 2));
            minutes = Int16.Parse(savedTimer.Substring(2, 2));
            seconds = Int16.Parse(savedTimer.Substring(4, 2));
            UpdateTimer();
            timer.Enabled = true;
        }
    }
}