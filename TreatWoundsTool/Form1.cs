using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Reflection;

namespace TreatWoundsTool
{
    public partial class Form1 : Form
    {
        int difficulty = 1;
        bool wardMedic = true;
        bool riskySurgery = false;
        bool risking = false;
        int DC = 15;
        int partyCount = 6;
        int medicineBonus = 9;
        int totalMinutes = 0;
        int totalHours = 0;
        int permittableWounds = 3;
        int healBonus = 0;
        string output = "";

        int pauseInterval = 60;
        int intervalTime = 0;
        bool paused = false;

        int critCount = 0;
        int successCount = 0;
        int failCount = 0;
        int critFailCount = 0;

        Random r1 = new Random();
        Random r2 = new Random();
        Random r3 = new Random();
        Random r4 = new Random();

        private bool mouseDown;
        private Point lastLocation;

        System.Windows.Forms.Timer timer;

        public Form1()
        {
            InitializeComponent();
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler(timer_Tick);

            // INITIALIZE SETTINGS FROM SAVED SETTINGS

            // Get Names
            textBox1.Text = Properties.Settings.Default["p1name"].ToString();
            textBox2.Text = Properties.Settings.Default["p2name"].ToString();
            textBox3.Text = Properties.Settings.Default["p3name"].ToString();
            textBox4.Text = Properties.Settings.Default["p4name"].ToString();
            textBox5.Text = Properties.Settings.Default["p5name"].ToString();
            textBox6.Text = Properties.Settings.Default["p6name"].ToString();

            // Get Difficulty
            difficulty = Convert.ToInt32(Properties.Settings.Default["difficulty"]);
            switch (difficulty) 
            {
                case 2:
                    radioButton3.Checked = true;
                    break;
                case 3:
                    radioButton2.Checked = true;
                    break;
                case 4:
                    radioButton1.Checked = true;
                    break;
                default:
                    radioButton4.Checked = true;
                    break;
            }

            // Get Medicine Bonus
            medicineBonus = Convert.ToInt32(Properties.Settings.Default["medicineBonus"]);
            numericUpDown6.Value = medicineBonus;

            // Get Time Interval
            pauseInterval = Convert.ToInt32(Properties.Settings.Default["pauseInterval"]);
            numericUpDown7.Value = pauseInterval;

            // Get Permittable Wounds
            permittableWounds = Convert.ToInt32(Properties.Settings.Default["permittableWounds"]);
            numericUpDown9.Value = permittableWounds;

            // Get Feats
            wardMedic = (Boolean)Properties.Settings.Default["wardMedic"];
            checkBox1.Checked = wardMedic;
            riskySurgery = (Boolean)Properties.Settings.Default["riskySurgery"];
            checkBox4.Checked = riskySurgery;
        }

        void timer_Tick(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e) // Treat Wounds Button
        {

            // Reset variables to defaults:
            paused = false;
            button3.Size = new Size(437, 62);
            totalMinutes = 0;
            totalHours = 0;
            critCount = 0;
            successCount = 0;
            failCount = 0; 
            critFailCount = 0;
            intervalTime = 0;
            healing = true; // Healing has started.
            bool cancel = false;

            // Clear text box.
            outputBox.Text = "";
            output = "Treating wounds... \r\n\r\n\r\n";



            switch (difficulty) // Get Difficulty, Set Healing Bonuses and DC's
            {
                case 2:
                    DC = 20;
                    healBonus = 10;
                    break;
                case 3:
                    DC = 30;
                    healBonus = 30;
                    break;
                case 4:
                    DC = 40;
                    healBonus = 50;
                    break;
                default:
                    DC = 15;
                    break;
            }

            //partyCount = 6; // Set Partycount to 5

            int[] party = new int[partyCount];
            string[] name = new string[partyCount];
            ProgressBar[] bar = new ProgressBar[partyCount];

            // Get Player Names
            name[0] = textBox1.Text;
            name[1] = textBox2.Text;
            name[2] = textBox3.Text;
            name[3] = textBox4.Text;
            name[4] = textBox5.Text;
            name[5] = textBox6.Text;

            // Get Player Wounds
            party[0] = Convert.ToInt32(numericUpDown1.Value);
            party[1] = Convert.ToInt32(numericUpDown2.Value);
            party[2] = Convert.ToInt32(numericUpDown3.Value);
            party[3] = Convert.ToInt32(numericUpDown4.Value);
            party[4] = Convert.ToInt32(numericUpDown5.Value);
            party[5] = Convert.ToInt32(numericUpDown8.Value);

            // Get Progress Bars
            bar[0] = progressBar1;
            bar[1] = progressBar2;
            bar[2] = progressBar3; 
            bar[3] = progressBar4;
            bar[4] = progressBar5;
            bar[5] = progressBar6;

            // Prepare Progress Bars
            bar[0].Maximum = Convert.ToInt32(numericUpDown1.Value);
            bar[1].Maximum = Convert.ToInt32(numericUpDown2.Value);
            bar[2].Maximum = Convert.ToInt32(numericUpDown3.Value);
            bar[3].Maximum = Convert.ToInt32(numericUpDown4.Value);
            bar[4].Maximum = Convert.ToInt32(numericUpDown5.Value);
            bar[5].Maximum = Convert.ToInt32(numericUpDown8.Value);

            // Get Medicine Bonus
            medicineBonus = Convert.ToInt32(numericUpDown6.Value);

            // Get Party HP
            for (int i = 0; i < partyCount; i++)
            {

                // Repeat Medicine attempts until player is sufficiently healed.
                while (party[i] > permittableWounds)
                {
                    // If user has cancelled the healing.
                    if (cancel == true)
                    {
                        //output = "";
                        //outputBox.Text = output;
                        break;
                    }

                    // If a pause interval is activated:
                    if (pauseInterval != 0)
                    {
                        // If the current interval time is greater than or equal to the pause interval.
                        if (intervalTime >= pauseInterval)
                        {
                            // Update wounds counters.
                            numericUpDown1.Value = party[0];
                            numericUpDown2.Value = party[1];
                            numericUpDown3.Value = party[2];
                            numericUpDown4.Value = party[3];
                            numericUpDown5.Value = party[4];
                            numericUpDown8.Value = party[5];
                            output += "\r\nIt has been " + intervalTime + " minutes. Proceed unhindered?\r\n\r\n";

                            // Scroll text box down.
                            outputBox.SelectionStart = outputBox.TextLength;
                            outputBox.ScrollToCaret();

                            // Update progress bars.
                            bar[0].Value = (bar[0].Maximum - party[0]);
                            bar[1].Value = (bar[1].Maximum - party[1]);
                            bar[2].Value = (bar[2].Maximum - party[2]);
                            bar[3].Value = (bar[3].Maximum - party[3]);
                            bar[4].Value = (bar[4].Maximum - party[4]);
                            bar[5].Value = (bar[5].Maximum - party[5]);

                            paused = true;
                            //button3.Size = new Size(369, 62);
                            outputBox.Text = output; // Update Text Box.
                        }

                        // button4
                        if (paused == true)
                        {
                            // Update wounds counters.
                            numericUpDown1.Value = party[0];
                            numericUpDown2.Value = party[1];
                            numericUpDown3.Value = party[2];
                            numericUpDown4.Value = party[3];
                            numericUpDown5.Value = party[4];
                            numericUpDown8.Value = party[5];
                            // Scroll text box down.
                            outputBox.SelectionStart = outputBox.TextLength;
                            outputBox.ScrollToCaret();
                            string message = "It has been " + intervalTime + " minutes. Proceed unhindered?";
                            bool userResult = MyProgramtWindowsModeErr(message);
                            if (userResult == true)
                            {
                                cancel = true;
                                // Scroll text box down.
                                outputBox.SelectionStart = outputBox.TextLength;
                                outputBox.ScrollToCaret();
                                //output = "";
                                //outputBox.Text = output;
                                break;
                            }

                            // Scroll text box down.
                            outputBox.SelectionStart = outputBox.TextLength;
                            outputBox.ScrollToCaret();
                            intervalTime -= pauseInterval;
                            paused = false;
                        }

                    }


                    outputBox.Text = output; // Update Text Box.

                    // Roll A Medicine Check
                    int result = 1; // 0 = Critical Failure, 1 = Failure, 2 = Success, 3 = Critical Success
                    int healing = 0;
                    int damage = 0; // Damage from Risky Surgery.
                    risking = false;
                    totalMinutes += 10;
                    intervalTime += 10;

                    int medicineRoll = r1.Next(1, 21);
                    medicineRoll = medicineRoll + medicineBonus;

                    // Activate Risky Surgery if feat is enabled.
                    if (riskySurgery == true)
                    {
                        risking = true;
                        medicineRoll += 2;
                    }
                    else
                    {
                        risking = false;
                    }

                    // Get Treat Wounds Check Result
                    if (medicineRoll >= DC + 10)
                    {
                        // Critical Success
                        result = 3;
                    }
                    else
                    if (medicineRoll >= DC)
                    {
                        // Success
                        if (risking == true)
                        {
                            result = 3; // Success is considered Critical Success when using Risky Surgery.
                        }
                        else
                        {
                            result = 2;
                        }
                    }
                    else
                    if (medicineRoll <= DC - 10)
                    {
                        // Success
                        result = 0;
                    }

                    // Subtract 1d8 if Risky Surgery is active.
                    if (risking == true)
                    {
                        damage = (r1.Next(1, 9));
                        party[i] = party[i] + damage;
                        output += "\r\nRISKY SURGERY - Automatic critical success on success.";
                        output += "\r\nDamaging " + name[i] + " for " + damage + " slashing damage. (" + party[i] + " wounds left.)";
                        outputBox.Text = output; // Update Text Box.
                    }

                    // Calculate Healing
                    switch (result)
                    {
                        // 0 = Critical Failure, 1 = Failure, 2 = Success, 3 = Critical Success
                        case 0:
                            healing = -1 * (r1.Next(1, 9));
                            critFailCount++;
                            Console.Write("\r\nCritical Failure (" + medicineRoll + " vs DC " + DC + ")");
                            output += "\r\nCritical Failure (" + medicineRoll + " vs DC " + DC + ")\r\n";
                            break;
                        case 1:
                            healing = 0;
                            failCount++;
                            Console.WriteLine("\r\nFailure (" + medicineRoll + " vs DC " + DC + ")");
                            output += "\r\nFailure (" + medicineRoll + " vs DC " + DC + ")\r\n";
                            break;
                        case 2:
                            healing = (r1.Next(1, 9) + r2.Next(1, 9)) + healBonus;
                            successCount++;
                            Console.Write("\r\nSuccess (" + medicineRoll + " vs DC " + DC + ")");
                            output += "\r\nSuccess (" + medicineRoll + " vs DC " + DC + ")\r\n";
                            break;
                        case 3:
                            healing = (r1.Next(1, 9) + r2.Next(1, 9) + r3.Next(1, 9) + r4.Next(1, 9)) + healBonus;
                            critCount++;
                            Console.Write("\r\nCritical Success (" + medicineRoll + " vs DC " + DC + ")");

                            output += "\r\nCritical Success (" + medicineRoll + " vs DC " + DC + ")\r\n";
                            break;
                        default:
                            healing = 0;
                            break;
                    }

                    outputBox.Text = output; // Update Text Box.

                    // Heal the wounds.
                    party[i] = party[i] - healing;
                    if (party[i] < 0) { party[i] = 0; }
                    if (result == 2 || result == 3)
                    {
                        Console.WriteLine("\r\nHealing " + name[i] + " for " + healing + ". (" + party[i] + " wounds left.)");
                        output += "Healing " + name[i] + " for " + healing + ". (" + party[i] + " wounds left.)\r\n";
                    }
                    if (result == 0)
                    {
                        Console.WriteLine("\r\nDamaging Player " + name[i] + " for " + healing + ". (" + party[i] + " wounds left.)");
                        output += "Damaging Player " + name[i] + " for " + healing + ". (" + party[i] + " wounds left.)\r\n";
                    }
                    // If Ward Medic is enabled, heal the lowest party member as well.
                    if ((i != (partyCount - 1)) && (wardMedic == true) && (healing > 0))
                    {

                        int pos = 0;
                        for (int j = 0; j < party.Length; j++)
                        {
                            if ((party[j] > party[pos]) && (party[j] > permittableWounds) && (party[j] != party[i])) { pos = j; }
                        }
                        if (party[pos] != party[i])
                        {
                            party[pos] = party[pos] - healing;
                            if (party[pos] < 0) { party[pos] = 0; }
                            Console.WriteLine("WARD MEDIC: Healing " + name[pos] + " for " + healing + " as well. (" + party[pos] + " wounds left.)");
                            output += "WARD MEDIC: Healing " + name[pos] + " for " + healing + " as well. (" + party[pos] + " wounds left.)\r\n";
                            outputBox.ScrollToCaret();
                        }
                    }
                    
                }
                
            }

            outputBox.Text = output; // Update Text Box.

            output += "\r\n\r\n";
            output += "The party has been healed!\r\n\r\n";

            for (int i = 0; i < partyCount; i++)
            {
                if (party[i] <= 0)
                {
                    Console.WriteLine(name[i] + " has been fully healed.");
                }
                else
                {
                    Console.WriteLine(name[i] + " still has " + party[i] + " wounds.");
                }
            }

            outputBox.Text = output; // Update Text Box.

            // Calculate Time
            while (totalMinutes >= 60)
            {
                totalMinutes = totalMinutes - 60;
                totalHours++;
            }

            // Output Results
            output += "\r\nThe healing process took " + totalHours + " Hours and " + totalMinutes + " Minutes.\r\n\r\n";

            if (critFailCount > 0)
            {
                output += "There were " + critFailCount + " critically failed Treat Wounds attempts.\r\n";
            }
            else
            {

            }

            if (failCount > 0)
            {
                output += "There were " + failCount + " failed Treat Wounds attempts.\r\n";
            }
            else
            {
                output += "There were no failed Treat Wounds attempts.\r\n";
            }

            if (successCount > 0)
            {
                output += "There were " + successCount + " successful Treat Wounds attempts.\r\n";
            }
            else
            {
                output += "There were no successful Treat Wounds attempts.\r\n";
            }

            if (critCount > 0)
            {
                output += "There were " + critCount + " critically successful Treat Wounds attempts!\r\n";
            }

            // Update wounds counters.
            numericUpDown1.Value = party[0];
            numericUpDown2.Value = party[1];
            numericUpDown3.Value = party[2];
            numericUpDown4.Value = party[3];
            numericUpDown5.Value = party[4];
            numericUpDown8.Value = party[5];

            outputBox.Text = output; // Update Text Box.

            healing = false; // Healing is over.

            // Update progress bars.
            bar[0].Value = (bar[0].Maximum - party[0]);
            bar[1].Value = (bar[1].Maximum - party[1]);
            bar[2].Value = (bar[2].Maximum - party[2]);
            bar[3].Value = (bar[3].Maximum - party[3]);
            bar[4].Value = (bar[4].Maximum - party[4]);
            bar[5].Value = (bar[5].Maximum - party[5]);

            // Scroll text box down.
            outputBox.SelectionStart = outputBox.TextLength;
            outputBox.ScrollToCaret();

            foreach (ProgressBar progBar in bar)
            {
                if (progBar.Value >= (progBar.Maximum - permittableWounds))
                {
                    progBar.Value = progBar.Maximum;
                }
            }


        } // End Treat Wounds


        private object CloneObject(object o)
        {
            Type t = o.GetType();
            PropertyInfo[] properties = t.GetProperties();


            Object p = t.InvokeMember("", System.Reflection.
                BindingFlags.CreateInstance, null, o, null);


            foreach (PropertyInfo pi in properties)
            {
                if (pi.CanWrite)
                {
                    pi.SetValue(p, pi.GetValue(o, null), null);
                }
            }
            return p;

        }


        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
            }
        }



        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        public bool MyProgramtWindowsModeErr(string msg)
        {

            if (DialogResult.Yes == MessageBox.Show(msg, "Time Interval", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                return false;
            }

            return true;
        }

        private void button1_Click(object sender, EventArgs e) // Close App Button
        {
            Environment.Exit(0);
        }

        private void button2_Click(object sender, EventArgs e) // Minimize App Button
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            difficulty = 1;
            Properties.Settings.Default["difficulty"] = difficulty;
            Properties.Settings.Default.Save();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            difficulty = 2;
            Properties.Settings.Default["difficulty"] = difficulty;
            Properties.Settings.Default.Save();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            difficulty = 3;
            Properties.Settings.Default["difficulty"] = difficulty;
            Properties.Settings.Default.Save();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            difficulty = 4;
            Properties.Settings.Default["difficulty"] = difficulty;
            Properties.Settings.Default.Save();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) // Ward Medic Toggle
        {
            if (checkBox1.Checked == true)
            {
                wardMedic = true;
                Properties.Settings.Default["wardMedic"] = checkBox1.Checked;
                Properties.Settings.Default.Save();
            } else
            {
                wardMedic = false;
                Properties.Settings.Default["wardMedic"] = checkBox1.Checked;
                Properties.Settings.Default.Save();
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e) // Risky Surgery Toggle
        {
            if (checkBox4.Checked == true)
            {
                riskySurgery = true;
                Properties.Settings.Default["riskySurgery"] = checkBox4.Checked;
                Properties.Settings.Default.Save();
            }
            else
            {
                riskySurgery = false;
                Properties.Settings.Default["riskySurgery"] = checkBox4.Checked;
                Properties.Settings.Default.Save();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e) // P1 Name
        {
            Properties.Settings.Default["p1name"] = textBox1.Text;
            Properties.Settings.Default.Save();
        }

        private void textBox2_TextChanged(object sender, EventArgs e) // P2 Name
        {
            Properties.Settings.Default["p2name"] = textBox2.Text;
            Properties.Settings.Default.Save();
        }

        private void textBox3_TextChanged(object sender, EventArgs e) // P3 Name
        {
            Properties.Settings.Default["p3name"] = textBox3.Text;
            Properties.Settings.Default.Save();
        }

        private void textBox4_TextChanged(object sender, EventArgs e) // P4 Name
        {
            Properties.Settings.Default["p4name"] = textBox4.Text;
            Properties.Settings.Default.Save();
        }

        private void textBox5_TextChanged(object sender, EventArgs e) // P5 Name
        {
            Properties.Settings.Default["p5name"] = textBox5.Text;
            Properties.Settings.Default.Save();
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e) // Medicine Score
        {
            medicineBonus = Convert.ToInt32(numericUpDown6.Value);
            Properties.Settings.Default["medicineBonus"] = medicineBonus;
            Properties.Settings.Default.Save();
        }

        private void Form1_MouseDown_1(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void numericUpDown7_ValueChanged(object sender, EventArgs e)
        {
            pauseInterval = Convert.ToInt32(numericUpDown7.Value);
            Properties.Settings.Default["pauseInterval"] = pauseInterval;
            Properties.Settings.Default.Save();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (paused == true)
            {
                intervalTime -= pauseInterval;
                paused = false;
                button3.Size = new Size(437, 62);
            }
        }

        private void numericUpDown1_Enter(object sender, EventArgs e)
        {
            numericUpDown1.Select(0, numericUpDown1.Text.Length);
        }

        private void numericUpDown2_Enter(object sender, EventArgs e)
        {
            numericUpDown2.Select(0, numericUpDown2.Text.Length);
        }

        private void numericUpDown3_Enter(object sender, EventArgs e)
        {
            numericUpDown3.Select(0, numericUpDown3.Text.Length);
        }

        private void numericUpDown4_Enter(object sender, EventArgs e)
        {
            numericUpDown4.Select(0, numericUpDown4.Text.Length);
        }

        private void numericUpDown5_Enter(object sender, EventArgs e)
        {
            numericUpDown5.Select(0, numericUpDown5.Text.Length);
        }

        private void numericUpDown8_Enter(object sender, EventArgs e)
        {
            numericUpDown8.Select(0, numericUpDown8.Text.Length);
        }

        private void numericUpDown1_Click(object sender, EventArgs e)
        {
            numericUpDown1.Select(0, numericUpDown1.Text.Length);
        }

        private void numericUpDown2_Click(object sender, EventArgs e)
        {
            numericUpDown2.Select(0, numericUpDown2.Text.Length);
        }

        private void numericUpDown3_Click(object sender, EventArgs e)
        {
            numericUpDown3.Select(0, numericUpDown3.Text.Length);
        }

        private void numericUpDown4_Click(object sender, EventArgs e)
        {
            numericUpDown4.Select(0, numericUpDown4.Text.Length);
        }

        private void numericUpDown5_Click(object sender, EventArgs e)
        {
            numericUpDown5.Select(0, numericUpDown5.Text.Length);
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            //GroupBox newPanel = (GroupBox)CloneObject(groupBox1);
            //newPanel.Location = new Point(10, 200);
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["p6name"] = textBox6.Text;
            Properties.Settings.Default.Save();
        }

        private void numericUpDown9_ValueChanged(object sender, EventArgs e)
        {
            permittableWounds = Convert.ToInt32(numericUpDown9.Value);
            Properties.Settings.Default["permittableWounds"] = permittableWounds;
            Properties.Settings.Default.Save();
        }
    }

    public static class ModifyProgressBarColor
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr w, IntPtr l);
        public static void SetState(this ProgressBar pBar, int state)
        {
            SendMessage(pBar.Handle, 1040, (IntPtr)state, IntPtr.Zero);
        }
    }
}
