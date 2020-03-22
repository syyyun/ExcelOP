using System;
using System.Windows.Forms;
using System.IO;
using System.Configuration;

namespace CSV2ExcelCreateTool
{
    public partial class Form1 : Form
    {
        // フォームロード
        public Form1()
        {
            InitializeComponent();
        }

        //入出力ファイル選択ボタン
        private void btn_FileSerect_Click(object sender, EventArgs e)
        {
            SerectFileDisplay(((Button)sender).Name);
        }

        //選択ファイルをテキストボックスに表示
        public void SerectFileDisplay(object BtnName)
        {
            OpenFileDialog ofDialog = new OpenFileDialog();

            // デフォルトのフォルダを指定
            ofDialog.InitialDirectory = ConfigurationManager.AppSettings["SELECTPATH"];

            //ダイアログのタイトルを指定
            ofDialog.Title = "ExcelFileCreateTool";

            if (ofDialog.ShowDialog() == DialogResult.OK)
            {
                if (BtnName.ToString() == "btn_InFileSerect")
                {
                    textBox1.Text = ofDialog.FileName;
                }
                else
                {
                    textBox2.Text = ofDialog.FileName;
                }
            }
            ofDialog.Dispose();
        }


        //実行ボタン
        private void btnCreateExl_Click(object sender, EventArgs e)
        {
            if (File.Exists(textBox1.Text) == false || File.Exists(textBox2.Text) == false)
            {
                //ファイル存在チェック
                if (File.Exists(textBox1.Text) == false)
                {
                    MessageBox.Show("入力ファイルが存在しません", "ExcelFileCreateTool", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this.textBox1.Focus();
                    return;
                }
                if (File.Exists(textBox2.Text) == false)
                {
                    MessageBox.Show("出力ファイルが存在しません", "ExcelFileCreateTool", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this.textBox2.Focus();
                }
                return;
            }

            //出力処理実行中の操作不可
            this.Enabled = false;

            //入力ファイルを取込、出力ファイルを作成
            FileOP op = new FileOP();
            var ret = op.ReadEX(textBox1.Text,textBox2.Text);

            this.Enabled = true;

            if (ret == "1")
            {
                MessageBox.Show("処理が完了しました", "ExcelFileCreateTool", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.textBox1.Focus();
            }
            else
            {
                MessageBox.Show("出力に失敗しました。\r\n \r\n" + ret, "ExcelFileCreateTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //終了ボタン
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close(); 
        }

    }
}
