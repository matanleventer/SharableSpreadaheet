using System;
using System.Threading;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;

namespace q1
{
    class SharableSpreadaheet
    {
        int numRows;
        int numCols;
        String[,] sheet;
        Semaphore_sheet sem;
        Mutex mut1 = new Mutex(); // does anyone write?
        Semaphore mut2 = new Semaphore(0,1); // does anyone read?
        int Rcounter = 0;

        public SharableSpreadaheet(int nRows1, int nCols1)
        {
            // construct a nRows*nCols spreadsheet
            this.numRows = nRows1;
            this.numCols = nCols1;
            int numUsers = nCols1 * nRows1;
            this.sem = new Semaphore_sheet(0, numUsers);
            this.sheet = new String[numRows, numCols];
            mut2.Release();
        }
        public int get_row()
        {
            return numRows;
        }
        public int get_col()
        {
            return numCols;
        }
        public void start_read()
        {
            mut1.WaitOne();
            Rcounter++;
            if (Rcounter == 1)
            {
                mut1.ReleaseMutex();
                mut2.WaitOne(); // writers need to wait
            }
            else 
            {
                mut1.ReleaseMutex();
            }
        }
        public void end_read()
        {
            mut1.WaitOne();
            Rcounter--;
            if (Rcounter == 0)
            {
                mut1.ReleaseMutex();
                mut2.Release(); //no more readers here
            }
            else
            { 
                mut1.ReleaseMutex();
            }
        }
        public String getCell(int row, int col)
        {
            // return the string at [row,col]
            start_read();
            String ret = sheet[row-1, col-1];
            end_read();
            return ret;
        }
        public bool setCell(int row, int col, String str)
        {
            // set the string at [row,col]
            mut2.WaitOne(); // does anyone reads?
            mut1.WaitOne(); // not allowing readers to read
            sheet[row-1, col-1] = str; //TODO check locations
            mut1.ReleaseMutex();
            mut2.Release();
            return true;
        }
        public bool searchString(String str, ref int row, ref int col)
        {
            // search the cell with string str, and return true/false accordingly.
            // stores the location in row,col.
            // return the first cell that contains the string (search from first row to the last row)
            start_read();
            sem.Wait();
            for (int i = 0; i < numRows; i++)
                for (int j = 0; j < numCols; j++)
                    if (sheet[i, j]==str)
                    {
                        row = i+1;
                        col = j+1;
                        sem.Release();
                        end_read();
                        return true;
                    }
            sem.Release();
            end_read();
            return false;
        }
        public bool exchangeRows(int row1, int row2)
        {

            // exchange the content of row1 and row2
            mut2.WaitOne(); // does anyone reads?
            mut1.WaitOne(); // not allowing readers to read
            String[] exchange = new String[numCols];
            for (int i = 0; i < numCols; i++)
                exchange[i] = (sheet[row1-1, i]);
            for (int j = 0; j < numCols; j++)
                sheet[row1-1, j] = (sheet[row2-1, j]);
            for (int k = 0; k < numCols; k++)
                sheet[row2-1, k] = (exchange[k]);
            mut1.ReleaseMutex();
            mut2.Release();
            return true;
        }
        public bool exchangeCols(int col1, int col2)
        {
            // exchange the content of col1 and col2
            mut2.WaitOne(); // does anyone reads?
            mut1.WaitOne(); // not allowing readers to read
            String[] exchange = new String[numRows];
            for (int i = 0; i < numRows; i++)
                exchange[i] = (sheet[i, col1-1]);
            for (int j = 0; j < numRows; j++)
                sheet[j, col1-1] = (sheet[j, col2-1]);
            for (int k = 0; k < numRows; k++)
                sheet[k, col2-1] = (exchange[k]);
            mut1.ReleaseMutex();
            mut2.Release();
            return true;
        }
        public bool searchInRow(int row, String str, ref int col)
        {
            // perform search in specific row
            start_read();
            sem.Wait();
            for (int i = 0; i < numCols; i++)
                if (sheet[row-1, i]==str)
                {
                    col = i+1;
                    end_read();
                    sem.Release();
                    return true;
                }
            end_read();
            sem.Release();
            return false;
        }
        public bool searchInCol(int col, String str, ref int row)
        {
            // perform search in specific col
            start_read();
            sem.Wait();
            for (int i = 0; i < numRows; i++)
                if (sheet[i, col-1]==str)
                {
                    row = i+1;
                    end_read();
                    sem.Release();
                    return true;
                }
            end_read();
            sem.Release();
            return false;
        }

        public bool searchInRange(int col1, int col2, int row1, int row2, String str, ref int row, ref int col)
        {
            // perform search within spesific range: [row1:row2,col1:col2] 
            //includes col1,col2,row1,row2
            start_read();
            sem.Wait();
            for (int i = row1-1; i <= row2-1; i++)
                for (int j = col1-1; j <= col2-1; j++)
                    if (sheet[i, j]==str)
                    {
                        row = i+1;
                        col = j+1;
                        end_read();
                        sem.Release();
                        return true;
                    }
            end_read();
            sem.Release();
            return false;
        }
        public bool addRow(int row1)
        {
            //add a row after row1
            //add a row after row1
            mut2.WaitOne(); // does anyone reads?
            mut1.WaitOne(); // not allowing readers to read
            String[,] exchange1 = new string[numRows + 1, numCols];
            for (int i = 0; i < numRows + 1; i++)
                for (int j = 0; j < numCols; j++)
                    if (i < row1)
                        exchange1[i, j] = sheet[i, j];
                    else if (i > row1)
                        exchange1[i, j] = sheet[i - 1, j];
            numRows++;
            this.sheet = exchange1;
            mut1.ReleaseMutex();
            mut2.Release();
            return true;
        }
        public bool addCol(int col1)
        {
            mut2.WaitOne(); // does anyone reads?
            mut1.WaitOne(); // not allowing readers to read
            String[,] exchange2 = new string[numRows, numCols + 1];
            for (int i = 0; i < numRows; i++)
                for (int j = 0; j < numCols+1; j++)
                    if (j < col1)
                        exchange2[i, j] = sheet[i, j];
                    else if (j > col1)
                        exchange2[i, j] = sheet[i, j - 1];
            numCols++;
            this.sheet = exchange2;
            mut1.ReleaseMutex();
            mut2.Release();
            return true;
        }
        public void getSize(ref int nRows, ref int nCols)
        {
            // return the size of the spreadsheet in nRows, nCols
            start_read();
            nRows = this.numRows;
            nCols = this.numCols;
            end_read();
        }
        public bool setConcurrentSearchLimit(int nUsers)
        {
            // this function aims to limit the number of users that can perform the search operations concurrently.
            // The default is no limit. When the function is called, the max number of concurrent search operations is set to nUsers. 
            // In this case additional search operations will wait for existing search to finish.
            // TODO return false if number of current users is bigger than nUsers.
            if (Rcounter <= nUsers) { 
                this.sem.setMax(nUsers);
                return true;
            }
            return false;
        }

        public bool save(String fileName)
        {
            // save the spreadsheet to a file fileName.
            // you can decide the format you save the data. There are several options.
            mut2.WaitOne(); // does anyone reads?
            mut1.WaitOne(); // not allowing readers to read
            using (StreamWriter ofile = new(fileName))
            {
                for (int i = 0; i < numRows; i++)
                {
                    String text = "";
                    for (int j = 0; j < numCols; j++)
                    {
                        text += sheet[i, j] + ",";
                    }
                    ofile.WriteLine(text);
                }
            }
            mut1.ReleaseMutex();
            mut2.Release();
            return true;
        }
        public bool load(String fileName)
        {
            // load the spreadsheet from fileName
            // replace the data and size of the current spreadsheet with the loaded data

            StreamReader st = new StreamReader(fileName + ".csv");
            var lines = new List<string[]>();
            int lineCounter = 0;
            
            while (!st.EndOfStream)
            {
                string[] Line = st.ReadLine().Split(',');
                lines.Add(Line);
                lineCounter ++;
            }
            string[,] loaded = new string[lineCounter, lines[0].Length];
            for (int i = 0; i < lineCounter; i++)
            {
                for (int j = 0; j < lines[0].Length; j++)
                {
                    loaded[i, j] = lines[i][j];
                }
            }
            this.sheet = loaded;
            this.numRows = lineCounter;
            this.numCols = lines[0].Length;
            return true;
        }
    }
}
