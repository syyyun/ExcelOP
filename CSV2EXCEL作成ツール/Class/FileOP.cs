using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace CSV2ExcelCreateTool
{
    public class FileOP
    {
        enum ExcelCol
        {
            A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z, AA, AB, AC, AD, AE, AF, AG, AH, AI, AJ, AK, AL, AM, AN, AO, AP, AQ, AR, AS, AT, AU, AV, AW, AX, AY, AZ, BA, BB, BC, BD, BE, BF, BG, BH, BI, BJ, BK, BL, BM, BN, BO, BP, BQ, BR, BS, BT, BU, BV, BW, BX, BY, BZ, CA, CB, CC, CD, CE, CF, CG, CH, CI, CJ, CK, CL, CM, CN, CO, CP, CQ, CR, CS, CT, CU, CV, CW, CX, CY, CZ, DA, DB, DC, DD, DE, DF, DG, DH, DI, DJ
        }

        //入力ファイル（CSV）を読込
        public List<string[]> ReadCsvData(string inFile)
        {
            List<string[]> lists = new List<string[]>();

            using (StreamReader sr = new StreamReader(inFile, System.Text.Encoding.GetEncoding("shift_jis")))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    var values = line.Split(',').Select(v => v.Trim(new char[] { '"' })).ToArray();
                    lists.Add(values);
                }
            }
            return lists;
        }

        //出力ファイル（Excel）の作成
        public string ReadEX(string sInFile, string sOutFile)
        {
            Application oXL = null;
            Workbooks oWBs  = null;
            Workbook oWB    = null;
            Sheets oWS      = null;
            Worksheet oWS1  = null;
            Range oRng1     = null;
            Range oRng2     = null;

            try
            {
                oXL = new Application();
                oXL.Visible = false;

                oWBs = oXL.Workbooks;
                oWB = oWBs.Open(sOutFile);

                oWS = oWB.Sheets;
                oWS1 = oWS[1];

                //シートの保護を解除
                oWS1.Unprotect();

                //入力ファイルからデータ取込
                var slist = ReadCsvData(sInFile);

                //二次元配列(プロットするデータ格納用)
                object[,] pastingValues1 = new object[2, 29];     // 1行28列イメージ（タイトル用）
                object[,] pastingValues2 = new object[601, 29];   // 600行28列イメージ（データ部用）

                //入力ファイル名が指定コード形式（XX-XXXXXXX）の場合、プロット
                var inFileName = Path.GetFileNameWithoutExtension(sInFile);
                if (Regex.IsMatch(inFileName, @"^\d{2}-\d{7}$"))
                {
                    pastingValues1[0, (int)ExcelCol.B] = inFileName;
                }

                //出力行移動用カウンター
                int i = 0;

                //ループ１回目（項目名）は出力しないためスキップ
                foreach (string[] str in slist.Skip(1))
                {
                    if (str[0] == "-1")
                    {
                        pastingValues1[0, (int)ExcelCol.I] = str[3].ToString();     //名称

                        pastingValues2[0, (int)ExcelCol.AB] = (decimal.Parse(str[11]) * 100).ToString();   //実績進捗率×100
                    }
                    else
                    {
                        //強制文字列出力のため"'"付与
                        pastingValues2[1 + i, (int)ExcelCol.B] = "'" + str[1].ToString();   //No.

                        var numP = ((str[1].ToString()).Where(v => v == '.')).Count();

                        //名称
                        if (numP == 0 && str[14].ToString().Equals("TRUE", StringComparison.OrdinalIgnoreCase))
                        {
                            pastingValues2[1 + i, (int)ExcelCol.E] = "*" + str[3].ToString();
                        }
                        else if (numP == 0 && str[14].ToString().Equals("FALSE", StringComparison.OrdinalIgnoreCase))
                        {
                            pastingValues2[1 + i, (int)ExcelCol.E] = str[3].ToString();
                        }

                        if (numP != 0)
                        {
                            pastingValues2[1 + i, (int)ExcelCol.E + numP] = str[3].ToString();
                        }

                        i++;
                    }
                }

                // 二次元配列のデータをExcelに貼付
                oWS1.Select(Type.Missing);
                oRng1 = oWS1.Range[oWS1.Cells[2, 1], oWS1.Cells[2, 28]];
                oRng2 = oWS1.Range[oWS1.Cells[5, 1], oWS1.Cells[600, 28]];
                
                oRng1.Value = pastingValues1;
                oRng2.Value = pastingValues2;

                //ブックの保護を有効化
                oWS1.Protect();

                //Excelアプリケーション終了
                oWB.Application.DisplayAlerts = false;
                oWB.Save();
                oWB.Close();

                oXL.Quit();
                
                return "1";
            }
            catch (Exception e)
            {
                return e.Message;
            }
            finally
            {
                //オブジェクトの開放
                if (oRng1 != null) Marshal.ReleaseComObject(oRng1);
                if (oRng2 != null) Marshal.ReleaseComObject(oRng2);
                if (oWS1 != null) Marshal.ReleaseComObject(oWS1);
                if (oWS != null) Marshal.ReleaseComObject(oWS);
                if (oWB != null) Marshal.ReleaseComObject(oWB);
                if (oWBs != null) Marshal.ReleaseComObject(oWBs);
                if (oXL != null) Marshal.ReleaseComObject(oXL);
            }
        }
    }
}
