using NCalc;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.String;

namespace CalculatorApp
{
    public partial class Form1 : Form
    {
        Dictionary<string, int> symbols = new Dictionary<string, int> { { "0", 0 }, { "1", 0 }, { "2", 0 }, { "3", 0 }, 
        { "+", 1 },{ "-", 1 },{ "*", 1 },{ "/", 1 }, { "(", 2 },{ ")", 2 }};
        int countBrackets = 0;
        bool minus = false;

        public Form1()
        {
            InitializeComponent();
        }
        
        private void buttonNumber_Click(object sender, EventArgs e)
        {
            var text = (sender as Button).Text;
            Action add = () => { textBox_Res.Text += text; minus = false; };

            if (symbols[text] == 0)
            {
                if (ValidateNum(text)) add();
            }
            else if (symbols[text] == 1)
            {
                if (ValidateSim(text)) add();
            }
            else if (symbols[text] == 2)
            {
                if (ValidateBracket(text)) add();
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            textBox_Res.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox_Res.Text == "") return;
                var regex = new Regex(@"\d*", RegexOptions.IgnoreCase);
                var matches = regex.Matches(textBox_Res.Text);
                var delta = 0;
                foreach (Match element in matches)
                {
                    if (element.Length != 0)
                    {
                        var decInt = ToDec(element.Value, 4).ToString();
                        textBox_Res.Text = textBox_Res.Text.Remove(element.Index - delta, element.Length)
                            .Insert(element.Index - delta, decInt);
                        delta += element.Length - decInt.Length;
                    }
                }
                var val = Convert.ToDouble(new Expression(textBox_Res.Text).Evaluate());
                var integ = (int) val;
                var fract = (int) ((val - integ) * 100000);
                textBox_Res.Text = ToGexInt(integ);
                textBox_Res.Text += "." + ToBinFrac(fract, fract.ToString().Length);
            }
            catch
            {
                MessageBox.Show("Error!");
            }
        }

        public static string ToBinFrac(double frac, int len)
        {
            var str = "";
            int c;
            var n = 0;
            while (n < len)
            {
                frac *= 4;
                c = Convert.ToInt32(Math.Truncate(frac));
                str = str + c;
                frac -= c;
                n++;
            }
            return str;
        }

        public static string ToGexInt(int dec)
        {
            var str = "";
            while (dec > 0)
            {
                str = Concat(Convert.ToString(dec % 4), str);
                dec = dec / 4;
            }
            return (str.Length == 0)? "0" : str;
        }

        private static long ToDec(string value, int fromBase)
        {
            const string table = "0123456789";
            long rank = 1, result = 0;
            for (var i = value.Length - 1; i >= 0; i--)
            {
                var index = table.IndexOf(value[i]);
                result += rank * index;
                rank *= fromBase;
            }
            return result;
        }

        private bool ValidateSim(string text)
        {
            var textBoxText = textBox_Res.Text;
            return textBoxText != "" && symbols[textBoxText[textBoxText.Length - 1].ToString()] != 1;
        }

        private bool ValidateNum(string text)
        {
            var textBoxText = textBox_Res.Text;
            var lastIndex = textBoxText.Length - 1;
            return lastIndex <= 0 || symbols[textBoxText[textBoxText.Length - 1].ToString()] != 0 || text != ")";
        }

        private bool ValidateBracket(string text)
        {
            var textBoxText = textBox_Res.Text;
            var lastIndex = textBoxText.Length - 1;
            countBrackets = (text == "(") ? ++countBrackets : --countBrackets;
            if (countBrackets < 0) { countBrackets++; return false; }
            if (lastIndex >= 0 && symbols[textBoxText[lastIndex].ToString()] == 0 && text == "(") return false;
            if (lastIndex >= 0 && symbols[textBoxText[lastIndex].ToString()] == 1 && text == ")") return false;
            return true;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            var textBoxText = textBox_Res.Text;
            var lastIndex = textBoxText.Length - 1;
            if (lastIndex > 0 && symbols[textBoxText[textBoxText.Length - 1].ToString()] == 1) return;
            if (minus)
            {
                textBox_Res.Text = textBoxText.Remove(lastIndex);
                minus = false;
            }
            else
            {
                textBox_Res.Text += "-";
                minus = true;
            }
        }
    }
}