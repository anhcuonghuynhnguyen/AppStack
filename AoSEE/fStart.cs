using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AoSEE
{
    public partial class fStart : Form
    {
        public fStart()
        {
            InitializeComponent();
            if (Global.color == "Default")
            {
                tbExp.ForeColor = Color.Black;
                tbPostfix.ForeColor = Color.Black;
                tbResult.ForeColor = Color.Black;
            }
            else if (Global.color == "Blue")
            {
                tbExp.ForeColor = Color.Blue;
                tbPostfix.ForeColor = Color.Blue;
                tbResult.ForeColor = Color.Blue;
            }
            else
            {
                tbExp.ForeColor = Color.Red;
                tbPostfix.ForeColor = Color.Red;
                tbResult.ForeColor = Color.Red;
            }
            if (Global.font== "Arial")
            {
                tbExp.Font = new Font("Arial", 15); 
                tbPostfix.Font = new Font("Arial", 15);
                tbResult.Font = new Font("Arial", 15);
            } 
            else
            {
                tbExp.Font = new Font("Time New Romans", 15);
                tbPostfix.Font = new Font("Time New Romans", 15);
                tbResult.Font = new Font("Time New Romans", 15);
            }
        }
        // Stack Object
        public class Node
        {
            public Node next;
            public object data;
        }
        public class MyStack
        {
            public Node top;
            public bool IsEmpty()
            {
                return top == null;
            }
            public void Push(object ele)
            {
                Node n = new Node();
                n.data = ele;
                n.next = top;
                top = n;
            }
            public Node Pop()
            {
                if (top == null)
                    return null;
                Node d = top;
                top = top.next;
                return d;
            }
        }

        // Function Event
        // Event button Exit
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Event Click Calculate Button
        private void btnCal_Click(object sender, EventArgs e)
        {
            tbResult.Text = string.Empty;
            string exp = tbExp.Text;
            if (check_Expression(exp))
            {
                tbResult.Text = CalculateExp(exp).ToString();
                tbPostfix.Text = infixToPostfix(exp.Split());
                SavetoFile(exp, tbResult.Text, tbPostfix.Text);
            }
            else
            {
                MessageBox.Show("The expression is not valid, Please check again!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Event Click Delete Button
        private void btnDelete_Click(object sender, EventArgs e)
        {
            tbExp.Text = string.Empty;
            tbResult.Text = string.Empty;
            tbPostfix.Text = string.Empty;
        }
        private void btnDelete_MouseHover(object sender, EventArgs e)
        {
            btnDelete.BackgroundImage = ResourcePic.HoverExit;
            btnDelete.ForeColor = Color.White;
        }
        private void btnDelete_MouseLeave(object sender, EventArgs e)
        {
            btnDelete.BackgroundImage = ResourcePic.btnExit;
            btnDelete.ForeColor = Color.Red;
        }

        // Event Click Copy Button
        private void btnCopyRe_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbResult.Text))
            {
                Clipboard.SetText(tbResult.Text);
            }
        }

        private void btnCopyPost_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbPostfix.Text))
            {
                Clipboard.SetText(tbPostfix.Text);
            }
        }

        // Event type expression
        private void tbExp_TextChanged(object sender, EventArgs e)
        {
            string exp = tbExp.Text;
            if (check_Expression(exp))
            {
                lbValid.Text = "The expresion is valid";
                btnCal.BackgroundImage = ResourcePic.HoverStart;
            }
            else
            {
                lbValid.Text = "The expression is not valid";
                btnCal.BackgroundImage = ResourcePic.btnStart1;
            }
        }

        // Event hover button help

        private void btnQues_MouseHover(object sender, EventArgs e)
        {
            tooltipHover.Show("Help", btnQues);
        }

        // Function Program
        static bool check_Expression(string exp)
        {
            string[] arrExp = exp.Split(' ');
            if ((IsNumber(arrExp[0]) || arrExp[0] == "(") && arrExp[0].Length != 0)
            {
                int i = 1;
                bool checkBracket = false;
                while (i < arrExp.Length)
                {
                    if (i == arrExp.Length - 1)
                    {
                        if (arrExp[i].Length == 0)
                        {
                            return false;
                        }
                        else if (IsNumber(arrExp[i]))
                        {
                            break;
                        }
                        else if (arrExp[i] == ")" && checkBracket)
                        {
                            checkBracket = false;
                            break;
                        }
                        return false;
                    }
                    else if (arrExp[i] == "(" && compare(arrExp[i - 1]) != -1 && IsNumber(arrExp[i + 1]))
                    {
                        i += 1;
                        checkBracket = true;
                        continue;
                    }
                    else if (arrExp[i] == ")" && compare(arrExp[i + 1]) != -1 && IsNumber(arrExp[i - 1]) && checkBracket)
                    {
                        i += 1;
                        checkBracket = false;
                        continue;
                    }
                    else if (IsNumber(arrExp[i]) && (compare(arrExp[i - 1]) != -1 || arrExp[i - 1] == "(") && (compare(arrExp[i + 1]) != -1 || arrExp[i + 1] == ")"))
                    {
                        i += 1;
                        continue;
                    }
                    else if (compare(arrExp[i]) != -1 && (IsNumber(arrExp[i - 1]) || arrExp[i - 1] == ")") && (IsNumber(arrExp[i + 1]) || arrExp[i + 1] == "("))
                    {
                        i += 1;
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
                if (checkBracket == false && i != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        // Functions are used to process algorithms 
        static bool IsNumber(string pValue)
        {
            foreach (Char c in pValue)
            {
                if (!Char.IsDigit(c))
                    return false;
            }
            return true;
        }

        static float evaluateExp(string[] postFix)
        {
            MyStack ms = new MyStack();
            float result = 0;
            for (int i = 0; i < postFix.Length; i++)
            {
                string c = postFix[i];

                if (IsNumber(c))
                {
                    ms.Push(c);
                }
                else
                {
                    float operand1 = float.Parse((string)ms.Pop().data);
                    float operand2 = float.Parse((string)ms.Pop().data);

                    if (c == "+")
                    {
                        ms.Push((operand1 + operand2).ToString());
                    }
                    else if (c == "-")
                    {
                        ms.Push((operand2 - operand1).ToString());
                    }
                    else if (c == "*")
                    {
                        ms.Push((operand1 * operand2).ToString());
                    }
                    else if (c == "/")
                    {
                        ms.Push((operand2 / operand1).ToString());
                    }
                    else if (c == "^")
                    {
                        ms.Push(Math.Pow(operand2, operand1).ToString());
                    }
                }
            }

            result = float.Parse((string)ms.Pop().data);
            return result;
        }

        static int compare(string c)
        {
            switch (c)
            {
                case "+":
                case "-":
                    return 1;

                case "*":
                case "/":
                    return 2;

                case "^":
                    return 3;
            }
            return -1;
        }

        static string infixToPostfix(string[] inFix)
        {
            MyStack ms = new MyStack();
            string postFix = "";
            for (int i = 0; i < inFix.Length; i++)
            {
                string c = inFix[i];

                if (IsNumber(c))
                {
                    postFix += (c + " ");
                }
                else if (c == "(")
                {
                    ms.Push(c);
                }
                else if (c == ")")
                {
                    while (!ms.IsEmpty() && (string)ms.top.data != "(")
                    {
                        postFix += (ms.Pop().data + " ");
                    }

                    if (!ms.IsEmpty() && (string)ms.top.data != "(")
                    {
                        return "Invalid Expression";
                    }
                    else
                    {
                        ms.Pop();
                    }
                }
                else
                {
                    while (!ms.IsEmpty() && compare(c) <= compare((string)ms.top.data))
                    {
                        postFix += (ms.Pop().data + " ");
                    }
                    ms.Push(c);
                }
            }

            while (!ms.IsEmpty())
            {
                postFix += (ms.Pop().data + " ");
            }
            return postFix.TrimEnd();
        }
        // Result Here !!!
        static float CalculateExp(string exp)
        {
            float result = 0;
            string[] arrExp = exp.Split(' ');
            string str = infixToPostfix(arrExp);
            string[] arrStr = str.Split(' ');
            result = evaluateExp(arrStr);
            return result;
        }

        // Functions are used to crate, read, write to files 
        static void Write2File(ArrayList ct, String path)
        {
            if (File.Exists(path))
            {
                StreamWriter sw = File.AppendText(path);
                for (int i = 0; i < ct.Count; i++)
                    sw.WriteLine(ct[i]);
                sw.Close();
            }
            else
            {
                StreamWriter sw = File.CreateText(path);
                for (int i = 0; i < ct.Count; i++)
                    sw.WriteLine(ct[i]);
                sw.Close();
            }
        }

        static void SavetoFile(string exp, string result, string postfix)
        {
            string path = Global.path;
            if (!File.Exists(path))
            {
                FileStream fs = File.Create(path);
                fs.Close();
            }

            ArrayList wt = new ArrayList();
            string time = DateTime.Now.ToString("HH:mm:ss");
            string day = DateTime.Today.ToString("dd-MM-yyyy");
            wt.Add($"{time} - {day} : \n\tExpression: {exp} \n\tResult: {result} \n\tPostFix: {postfix}\n");
            Write2File(wt, path);
        }

        private void btnQues_Click(object sender, EventArgs e)
        {
            fHelp fhelp = new fHelp();
            this.Hide();
            fhelp.ShowDialog();
            this.Show();
        }
    }
}
