using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGui.Datatypes
{
    public static class Spreadsheet
    {
        public class Config
        {
            public char Delimiter = ',';
            public char OpenQuote = '\"';
            public char CloseQuote = '\"';

            public static readonly Config Default = new Config();
        }

        public static Spreadsheet<T> Read<T>( string fileName, bool hasRowNames = true, bool hasColNames = true, Converter<string, T>  converter = null, ProgressDelegate progressReporter = null )
        {
            return Spreadsheet<T>.Read( fileName, hasRowNames, hasColNames, converter, progressReporter );
        }    

        public delegate bool ProgressDelegate( int rowIndex, int numRows );

        public static string WriteFields( IEnumerable x, Config config = null)
        {
            config = config ?? Config.Default;
            StringBuilder result = new StringBuilder();

            foreach (string xx in x)
            {
                string s = Convert.ToString( xx );

                if (result.Length != 0)
                {
                    result.Append( config.Delimiter );
                }

                if (s.Contains( config.Delimiter ) || s.Contains( config.OpenQuote ) || s.Contains(config.CloseQuote ) || s.StartsWith( " " ) || s.EndsWith( " " ))
                {
                    result.Append( "\"" );  
                    result.Append( s.Replace( config.CloseQuote.ToString(), config.CloseQuote.ToString() + config.CloseQuote ) );
                    result.Append( "\"" );
                }
                else
                {
                    result.Append( s );
                }
            }

            return result.ToString();
        }

        public static string[] ReadFields( string v, Config config = null )
        {                                     
            List<string> result = new List<string>();
            ParseFields( result, v, config );
            return result.ToArray();
        }

        public static int CountFields( string v, Config config = null )
        {   
            return ParseFields( null, v, config );
        }

        private static int ParseFields( List<string> result, string text, Config config = null )
        {
            config = config ?? Config.Default;

            const int START = 0;
            const int TEXT = 1;
            const int IN_QUOTES = 2;
            const int END_QUOTES = 3;

            int fieldStart = 0;
            int startBeforeQuotes = 0;
            int fieldEnd = 0;
            int stage = START;
            int numberOfFields = 0;
            bool needToRemoveDoubleQuotes = false;

            Debug.WriteLine("------------------------");

            for (int n = 0; n <= text.Length; n++)
            {
                char c;

                if (n == text.Length)
                {
                    c = config.Delimiter;
                }
                else
                {
                    c = text[n];
                }

                //Debug.WriteLine("");
                //Debug.Write( stage );
                //Debug.Write( " " );
                //Debug.Write( c );
                //Debug.Write( " " );

                if (c == config.Delimiter)
                {                    
                    if (stage != IN_QUOTES)
                    {
                        //Debug.Write( "end field " );

                        if (stage != END_QUOTES)
                        {
                            fieldEnd = n;
                        }

                        if (result != null)
                        {
                            string field = text.Substring( fieldStart, fieldEnd - fieldStart );

                            if (needToRemoveDoubleQuotes)
                            {
                                field = field.Replace( config.CloseQuote.ToString() + config.CloseQuote, config.CloseQuote.ToString() );
                            }

                            result.Add( field );
                        }

                        fieldStart = n + 1;
                        ++numberOfFields;
                        stage = START;
                    }
                }
                else if (c == config.OpenQuote && stage == START)
                {        
                    //Debug.Write( "start quote " );

                    startBeforeQuotes = fieldStart;
                    fieldStart = n + 1;
                    stage = IN_QUOTES;
                }
                else if (c == config.CloseQuote)
                {
                    if (stage == IN_QUOTES)
                    {
                        if (n != text.Length - 1 && text[n + 1] == config.CloseQuote)
                        {
                            //Debug.Write( "double close quote (ignored) " );

                            needToRemoveDoubleQuotes = true;
                        }
                        else
                        {
                            //Debug.Write( "close quote " );

                            stage = END_QUOTES;
                            fieldEnd = n;
                        }
                    }
                }
                else
                {
                    if (stage == END_QUOTES)
                    {
                        //Debug.Write( "undo quotes " );

                        fieldStart = startBeforeQuotes; // undo quotes
                        stage = TEXT;
                    }
                    else if (stage == START)
                    {
                        //Debug.Write( "start text " );

                        fieldStart = n;
                        stage = TEXT;
                    }
                }
            }

            return numberOfFields;
        }
    }

    public class Spreadsheet<T>
    {
        public readonly string Title;
        public readonly string[] RowNames;
        public readonly string[] ColNames;
        public readonly T[,] Data;
        public readonly int NumRows;
        public readonly int NumCols;

        private Spreadsheet( string title, string[] rowNames, string[] colNames, T[,] data, int numRows, int numCols )
        {
            this.Title = title;
            this.RowNames = rowNames;
            this.ColNames = colNames;
            this.Data = data;
            this.NumRows = numRows;
            this.NumCols = numCols;      
        }

        private static Converter<string, T> GetConverter()
        {
            if (typeof( T ) == typeof( double ))
            {
                return z => (T)(object)double.Parse( z );
            }
            else if (typeof( T ) == typeof( int ))
            {
                return z => (T)(object)int.Parse( z );
            }
            else if (typeof( T ) == typeof( string ))
            {
                return z => (T)(object)z;
            }
            else
            {
                throw new InvalidOperationException( "Error" );
            }
        }

        public T this[int row, int col]
        {
            get { return Data[row, col]; }
        }

        public static Spreadsheet<T> Read( string fileName, bool hasRowNames, bool hasColNames, Converter<string, T> converter = null, Spreadsheet.ProgressDelegate progressReporter = null )
        {
            if (string.IsNullOrWhiteSpace( fileName ))
            {
                throw new InvalidOperationException( "Filename is missing." );
            }

            string title = fileName;
            int NumRows = 0;
            int NumCols = 0;

            if (converter == null)
            {
                converter = GetConverter();
            }

            // INITIAL READ TO GET SIZE OF DATA   
            using (StreamReader sr = new StreamReader( fileName ))
            {   
                if (hasColNames)
                {
                    sr.ReadLine();
                }

                int fields = Spreadsheet.CountFields( sr.ReadLine() );

                NumRows++;

                NumCols = hasRowNames ? fields - 1 : fields;

                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();

                    if (line.Length == 0)
                    {
                        break;
                    }

                    NumRows++;
                }
            }

            T[,] Data = new T[NumRows, NumCols];
            string[] RowNames = new string[NumRows];
            string[] ColNames;

            using (StreamReader sr = new StreamReader( fileName ))
            {
                // READ OR CREATE COLUMN NAMES
                if (hasColNames)
                {
                    // First row name is blank
                    string[] colNameData = Spreadsheet.ReadFields( sr.ReadLine() );

                    if (hasRowNames)
                    {
                        ColNames = new string[colNameData.Length - 1];
                        Array.Copy( colNameData, 1, ColNames, 0, colNameData.Length - 1 );
                    }
                    else
                    {
                        ColNames = colNameData;
                    }

                    NumCols = ColNames.Length;
                }
                else
                {
                    ColNames = new string[NumCols];

                    for (int col = 0; col < NumCols; col++)
                    {
                        ColNames[col] = "C" + col;
                    }
                }

                Assert( ColNames.Length == NumCols, "The number of column names (" + ColNames.Length + ") is different from number of columns (" + NumCols + "). Check the CSV file for errors." );

                // READ DATA ENTRIES
                int rowIndex = 0;

                while (!sr.EndOfStream)
                {
                    string[] lineData = Spreadsheet.ReadFields( sr.ReadLine() );

                    if (lineData.Length == 1 && lineData[0] == "")
                    {
                        break;
                    }

                    int dataCol;

                    if (hasRowNames)
                    {
                        dataCol = 1;
                        RowNames[rowIndex] = lineData[0];
                    }
                    else
                    {
                        dataCol = 0;
                        RowNames[rowIndex] = "R" + rowIndex;
                    }

                    int colIndex = 0;

                    for (int c = dataCol; c < lineData.Length; c++)
                    {
                        Data[rowIndex, colIndex] = converter( lineData[c] );
                        colIndex++;
                    }

                    Assert( colIndex == NumCols, "The number of columns is different for row " + rowIndex + " of the CSV file \"" + title + "\". Check the CSV file for errors." );

                    if (progressReporter != null)
                    {
                        if (!progressReporter( rowIndex, NumRows ))
                        {
                            return null;
                        }
                    }

                    rowIndex++;
                }

                Assert( rowIndex == NumRows, "Did not load all data for the CSV file \"" + title + "\". Check the CSV file for errors." );
            }

            return new Spreadsheet<T>(title, RowNames, ColNames, Data, NumRows, NumCols);
        }  

        /// <summary>
        /// Gets the index of the column with any of the specified title(s).
        /// </summary>
        /// <param name="colTitles">One or more titles</param>
        public int FindColumn( params string[] colTitles )
        {
            return InternalFind( colTitles, ColNames, "column", true ); 
        }/// 

        [Obsolete]
        public int ColIndex( string colTitles )
        {
            string[] e = colTitles.Split( ',' );
            return FindColumn( e );
        }

        [Obsolete]
        public int OptionalColIndex( string colTitles )
        {
            string[] e = colTitles.Split( ',' );
            return TryFindColumn( e );
        }

        /// <summary>
        /// Gets the index of the column with any of the specified title(s).
        /// </summary>
        /// <param name="colTitles">One or more titles</param>
        public int TryFindColumn( params string[] colTitles )
        {
            return InternalFind( colTitles, ColNames, "column", false );
        }

        private int InternalFind( string[] tries, string[] exists, string method, bool throwOnError )
        {
            if (tries.Length == 0)
            {
                return -1;
            }

            int result = -1;

            foreach (string colTitle in tries)
            {
                int n = -1;

                for (int m = 0; m < exists.Length; m++)
                {
                    if (exists[m].ToUpper() == colTitle.ToUpper())
                    {
                        n= m;
                        break;
                    }
                }

                if (n != -1)
                {
                    if (result != -1)
                    {
                        // Multiple matches
                        RaiseError(tries, exists, method, "multiple" );
                    }

                    result = n;
                }
            }

            if (throwOnError && result==-1)
            {
                RaiseError( tries, exists, method, "no" );
            }

            return result;
        }

        private void RaiseError( string[] tried, string[] exists, string method, string problem )
        {
            throw new KeyNotFoundException( "Expected to find a " + method.ToUpper() + " with any of the titles {\"" + string.Join( "\", \"", tried ) + "\"} in the \"" + Title + "\" data but there are "+problem.ToUpper()+" matching " + method + "s in the array {\"" + string.Join( "\", \"", exists ) + "\"}. The names are not case sensitive. Check the CSV file for errors and make sure the settings are correct." );
        }

        public int TryFindRow( params string[] rowTitles )
        {
            return InternalFind( rowTitles, RowNames, "row", false );
        }

        public int FindRow( params string[] rowTitles )
        {
            return InternalFind( rowTitles, RowNames, "row", true );
        }

        private static void Assert( bool condition, string message )
        {
            if (!condition)
            {
                throw new FormatException( message );
            }
        }

        /// <summary>
        /// Copies a column into an array.
        /// </summary>                    
        public T[] CopyColumn( int colIndex )
        {
            T[] result = new T[NumRows];

            for (int row = 0; row < result.Length; row++)
            {
                result[row] = this[row, colIndex];
            }

            return result;
        }

        /// <summary>
        /// Copies a row into an array.
        /// </summary>                   
        public T[] CopyRow( int rowIndex )
        {
            T[] result = new T[NumCols];

            for (int col = 0; col < result.Length; col++)
            {
                result[col] = this[rowIndex, col];
            }

            return result;
        }
    }                             

    public static class MatrixExtensions
    {
        /// <summary>
        /// (MJR) (EXTENSION) String as integer.
        /// </summary>                                          
        public static int AsInteger( this Spreadsheet<string> self, int row, int col )
        {
            string v = self[row, col];

            int r;

            if (int.TryParse( v, out r ))
            {
                return r;
            }

            throw new FormatException( $"The string \"{v}\" at row {row} and column {col} in matrix \"{self.Title}\" is not convertable to integer." );
        }
    }
}
