using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProiectColectivGUI
{

    public partial class Form1 : Form
    {


        public Form1()
        {
            InitializeComponent();
        }

        static string tipVarDarian = "TIP_VAR_NOT_GIVEN", tipVar2Darian = "TIP_VAR_2_NOT_GIVEN", functieReadHoza = "FUNCTIE_READ_NOT_GIVEN";
        static string[] parametriiFaraValori = new string[10];
        static string[] valoriParametriiFaraValori = new string[10];
        static int indexValoriParametriiFaraValori = 0;
        static int indexParametriiFaraValori = 0;
        static string filePath, fileName;
        OpenFileDialog ofd = new OpenFileDialog();
        bool DBCOpen = false, RteOpen = false, inputOpen = false;

        public static class ParametriiPrompt
        {
            public static string ShowDialog(string text, string caption, string[] parameters)
            {
                Form ParametriiForm = new Form()
                {
                    Width = 500,
                    Height = 150,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    Text = caption,
                    StartPosition = FormStartPosition.CenterParent,
                    AutoSize = true,
                };
                Label label = new Label() {Text = text, AutoSize= true };
                TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
                Button confirm = new Button() { Text = "Confirm", Left = 350, Width = 100, Top = 100, DialogResult = DialogResult.OK };
                ListBox listBox = new ListBox() { Left = 50, Top = 70, AutoSize = true};

                for(int i = 0; i < parameters.Length ; i++)
                {
                    if(parameters[i] != null)
                    {
                        listBox.Items.Add(parameters[i]);
                    }
                    
                }


                listBox.MouseDoubleClick += (sender2, e2) =>
                {
                    if (listBox.SelectedItem != null)
                    {
                        Form inserareValoareParametru = new Form()
                        {
                            AutoSize = true
                        };
                        TextBox textBox1 = new TextBox() {Top = 10 };
                        Button buttonAddVal = new Button() {Top = 30 ,Text = "AddVal", DialogResult = DialogResult.OK};
                        
                        
                        buttonAddVal.Click += (sender1, e1) => {
                            Console.WriteLine("++++++++++++++++++=>" + textBox1.Text);
                            valoriParametriiFaraValori[indexValoriParametriiFaraValori++] = textBox1.Text;
                            inserareValoareParametru.Close();
                        };

                        inserareValoareParametru.Controls.Add(textBox1);
                        inserareValoareParametru.Controls.Add(buttonAddVal);
                        inserareValoareParametru.AcceptButton = buttonAddVal;

                        inserareValoareParametru.ShowDialog();
                    }
                };


                confirm.Click += (sender, e) => { ParametriiForm.Close();
                    Console.WriteLine("+++++++++++++=> valoriParamFaraVal");
                    for (int i = 0; i < valoriParametriiFaraValori.Length; i++)
                    {
                        if (valoriParametriiFaraValori[i] != null)
                        {
                            Console.WriteLine(i + " " + valoriParametriiFaraValori[i]);
                        }
                    }
                };
                ParametriiForm.Controls.Add(label);
                ParametriiForm.Controls.Add(confirm);
                ParametriiForm.Controls.Add(listBox);
                ParametriiForm.AcceptButton = confirm;

                

                return ParametriiForm.ShowDialog() == DialogResult.OK ? textBox.Text : "";
            }
        }

        private void DBCButton_Click(object sender, EventArgs e)
        {
            String openFile;
            ofd.Filter = "dbcFile|*.dbc";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                openFile = ofd.FileName;
                DBCLabel.Text = ofd.SafeFileName;
                DBCOpen = true;
            }
        }

        private void Rte_VdaButton_Click(object sender, EventArgs e)
        {
            String openFile;
            ofd.Filter = "vdaFile|*.h";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                openFile = ofd.FileName;
                Rte_VdaLabel.Text = ofd.SafeFileName;
                RteOpen = true;
            }
        }

        private void FileInputButton_Click(object sender, EventArgs e)
        {
            String openFile;
            ofd.Filter = "txtFile|*.txt";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                openFile = ofd.FileName;
                FileInputLabel.Text = ofd.SafeFileName;

                filePath = ofd.FileName;
                fileName = ofd.SafeFileName;
                inputOpen = true;
            }
        }



        private void GenerateButton_Click(object sender, EventArgs e)
        {
            int nrAnds = 0, nrOrs=0;
            string toSearch = "Empty", elementDeCautat = "";
            string functionName = "Vda_Calculate", variable, valoare, outputPath, functionType = "void";
            string[] vectorValori = new string[10];
            try
            {
                toSearch = File.ReadAllText(filePath);
            }
            catch (IOException ioe)
            {
                Console.WriteLine(ioe.ToString());
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine(ane.ToString());
            }

            Console.WriteLine(toSearch);

            Match matchElemDeCautat = Regex.Match(toSearch, "IF\\s*\"([a-zA-z0-9]+)\"\\s*");
            if (matchElemDeCautat.Success)
            {
                elementDeCautat = matchElemDeCautat.Groups[1].Value;
                Console.WriteLine("elem de cautat---------------->" + elementDeCautat);
            }
            else
            {
                Console.WriteLine("elem de cautat----------------------------------> NOT FOUND");
            }
               



            Match matchFunctionName = Regex.Match(toSearch, "THEN:\\s*\"\\w*_([a-zA-z0-9]+)\"\\s*");
            if (matchFunctionName.Success)
            {
                functionName = functionName + matchFunctionName.Groups[1].Value;
                Console.WriteLine("function name-------------->" + functionName);
            }
            else
                Console.WriteLine("function name-------------------------> NOT FOUND");

            Match matchVariable = Regex.Match(toSearch, "THEN:\\s*\"(\\w+)\"\\s*");
            if (matchVariable.Success)
            {
                variable = matchVariable.Groups[1].Value;
                Console.WriteLine("variable-------------->" + variable);
            }
            else
            {
                variable = "NOT FOUND";
                Console.WriteLine("variable------------------------->" + variable);
            }


            Match matchValoare = Regex.Match(toSearch, @"\'([0-9])");
            if (matchValoare.Success)
            {

                int i = 0;
                //valoare = matchValoare.Groups[1].Value;
                while (matchValoare.Success)
                {
                    vectorValori[i] = matchValoare.Groups[1].Value;
                    matchValoare = matchValoare.NextMatch();
                    i++;
                }


                /* Console.WriteLine("valoare-------------->" + valoare);
                 Console.WriteLine("valoare-------------->" + matchValoare.NextMatch().Groups[1].Value);
                 Console.WriteLine("valoare-------------->" + matchValoare.NextMatch().NextMatch().Groups[1].Value);
                */
                for (int j = 0; j < vectorValori.Length; j++)
                {
                    if (vectorValori[j] != null)
                        Console.WriteLine("valoare-------------->" + vectorValori[j]);
                }
            }
            else
            {
                valoare = "NOT FOUND";
                Console.WriteLine("valoare------------------------->" + valoare);
            }

            Match numarAnd = Regex.Match(toSearch, "AND");
            if (numarAnd.Success)
            {
                while (numarAnd.Success)
                {
                    nrAnds++;
                    numarAnd = numarAnd.NextMatch();
                }
            }
            Console.WriteLine("nrAnds------------------------->" + nrAnds);

            Match numarOr = Regex.Match(toSearch, "OR");
            if(numarOr.Success)
            {
                while(numarOr.Success)
                {
                    nrOrs++;
                    numarOr = numarOr.NextMatch();
                }
            }
            Console.WriteLine("nrOrs------------------------->" + nrOrs);


            Console.WriteLine("file path ------------->" + filePath);



            void singleCondition()
            {
                int k = 0;

                StreamWriter streamWriter = new StreamWriter(outputPath + "\\output1.c", false, Encoding.ASCII);

                streamWriter.Write(
                    functionType + " " + functionName + "(" + functionType + ")" + "\r\n" +
                    "{" + "\r\n" +
                    tipVarDarian + " " + variable + ";" + "\r\n" +
                    tipVar2Darian + " " + elementDeCautat + ";" + "\r\n" +
                    functieReadHoza + "(&" + elementDeCautat + ");" + "\r\n" +
                    "if(" + elementDeCautat + " == " + vectorValori[k++] + ")" + "\r\n" +
                    "{" + "\r\n" +
                    "\t" + variable + " = " + vectorValori[k++] + ";" + "\r\n" +
                    "}" + "\r\n" +
                    "else" + "\r\n" +
                    "{" + "\r\n" +
                    "\t" + variable + " = " + vectorValori[k++] + ";" + "\r\n" +
                    "}" + "\r\n" +
                    "}"
                    );
                streamWriter.Close();
            }

            void multipleConditions(string[] vectorTipuriParametrii, string[] vectorParametriiFaraValori)
            {
                int k = 0;

                StreamWriter streamWriter = new StreamWriter(outputPath + "\\output1.c", false, Encoding.ASCII);

                streamWriter.Write(
                   functionType + " " + functionName + "(");

                int i = 0;
                for( i = 0; i < vectorTipuriParametrii.Length-1; i++)
                {
                    if(vectorTipuriParametrii[i+1] != null)
                    {
                        streamWriter.Write(vectorTipuriParametrii[i] + " " + vectorParametriiFaraValori[i] + ",");
                    }
                }

                i = 0;
                while (vectorTipuriParametrii[i+1] != null)
                    i++;

                streamWriter.Write(vectorTipuriParametrii[i] + " " + vectorParametriiFaraValori[i] + ")" + "\r\n");

                streamWriter.Write(
                    "{" + "\r\n" +
                    tipVarDarian + " " + variable + ";" + "\r\n" +
                    tipVar2Darian + " " + elementDeCautat + ";" + "\r\n" +
                    functieReadHoza + "(&" + elementDeCautat + ");" + "\r\n" +
                    "if(" + elementDeCautat + " == " + vectorValori[k++] + ")" + "\r\n" +
                    "{" + "\r\n" +
                    "\t" + variable + " = " + vectorValori[k++] + ";" + "\r\n" +
                    "}" + "\r\n" +
                    "else" + "\r\n" +
                    "{" + "\r\n" +
                    "\t" + variable + " = " + vectorValori[k++] + ";" + "\r\n" +
                    "}" + "\r\n" +
                    "}"
                    );
                streamWriter.Close();
            }


            bool tempOk = false;
            if (DBCOpen == true && RteOpen == true && inputOpen == true)
            {
                outputPath = filePath.Substring(0, filePath.Length - ((fileName.Length) + 1));
                if (nrAnds == 0 && nrOrs == 0)
                {
                    try
                    {
                        Console.WriteLine("output path --------->" + outputPath);
                        try
                        {
                            //daca nu se gaseste vreun parametru in dbc sau vda_rte (folosind functiile de la Hoza si Darian)
                            if (tempOk == false)
                            {
                               
                                parametriiFaraValori[indexParametriiFaraValori++] = elementDeCautat;
                                
                                Console.WriteLine("++++++++++++=>ParamFaraVal");

                                for(int i = 0; i < parametriiFaraValori.Length; i++)
                                {
                                    if (parametriiFaraValori[i] != null)
                                        Console.WriteLine(i + " " + parametriiFaraValori[i]);
                                }

                                string promptValue = ParametriiPrompt.ShowDialog("Multiple unidentified variables have been found, please insert their values here",
                                    "Variable names needed",
                                    parametriiFaraValori);

                                try
                                {
                                    multipleConditions(valoriParametriiFaraValori, parametriiFaraValori);

                                }
                                catch (Exception exc)
                                {
                                    Console.WriteLine("Exception" + exc.Message);
                                }

                                tempOk = true;
                                

                            }
                            else
                            {
                                //daca parametrii sunt gasiti in dbc sau rte_vda (folosind functiile de la Hoza si Darian)
                               try
                               {
                                   singleCondition();
                               }
                               catch (Exception exc)
                               {
                                   Console.WriteLine("Exception" + exc.Message);
                               }
                            }
                        }
                        catch (FileNotFoundException fnfe)
                        {
                            Console.WriteLine(fnfe.ToString());
                            MessageBox.Show(
                            "File not found",
                            "Warning",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        }
                        catch (IOException ioe)
                        {
                            MessageBox.Show(
                            "Something went wrong",
                            "Warning",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                            Console.WriteLine(ioe.ToString());
                        }
                    }
                    catch (NullReferenceException nre)
                    {
                        MessageBox.Show(
                            "Something went wrong",
                            "Warning",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        Console.WriteLine(nre.ToString());
                    }
                }
            }
            else
            {
                MessageBox.Show(
                        "You must select all 3 files",
                        "Warning",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
            }

        }
    }
}
