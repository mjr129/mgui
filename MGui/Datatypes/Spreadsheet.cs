using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MGui.Helpers;

namespace MGui.Datatypes
{
    /// <summary>
    /// Spreadsheet and parsing utilities.
    /// </summary>
    public class SpreadsheetReader
{
        public delegate bool ProgressDelegate( int rowIndex, int numRows );

        public char Delimiter = ',';
        public char OpenQuote = '\"';
        public char CloseQuote = '\"';
        public bool HasRowNames = true;
        public bool HasColNames = true;
        public bool TolerantConversion = false;
        public ProgressDelegate Progress = null;
        public int WriteSpaces = 0;

        public static readonly SpreadsheetReader Default = new SpreadsheetReader();

        /// <summary>
        /// Obtains the default converter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tolerantConversion"></param>
        /// <returns></returns>
        private Converter<string, T> GetConverter<T>()
        {
            if (typeof( T ) == typeof( double ))
            {
                if (this.TolerantConversion)
                {
                    return z =>
                    {
                        double result;
                        return double.TryParse( z, out result ) ? (T)(object)result : default( T );
                    };
                }
                else
                {
                    return z => (T)(object)double.Parse( z );
                }
            }
            else if (typeof( T ) == typeof( int ))
            {
                if (this.TolerantConversion)
                {
                    return z =>
                    {
                        int result;
                        return int.TryParse( z, out result ) ? (T)(object)result : default( T );
                    };
                }
                else
                {
                    return z => (T)(object)int.Parse( z );
                }
            }
            else if (typeof( T ) == typeof( long ))
            {
                if (this.TolerantConversion)
                {
                    return z =>
                    {
                        long result;
                        return long.TryParse( z, out result ) ? (T)(object)result : default( T );
                    };
                }
                else
                {
                    return z => (T)(object)long.Parse( z );
                }
            }
            else if (typeof( T ) == typeof( Cell ))
            {
                return z=> (T)(object)new Cell( z );
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

        /// <summary>
        /// Delimiter (for writing only)
        /// Includes any prefixed spaces if necessary
        /// </summary>
        private string FullDelimiter
        {
            get
            {
                if (WriteSpaces != 0)
                {
                    return Delimiter + new string( ' ', WriteSpaces );
                }
                else
                {
                    return Delimiter.ToString();
                }
            }
        }

        public void Write( ISpreadsheet ss, string fileName, Func<object, string> converter = null )
        {
            using (StreamWriter sw = new StreamWriter( fileName ))
            {
                if (this.HasColNames)
                {
                    if (this.HasRowNames)
                    {
                        sw.Write( FullDelimiter );
                    }

                    sw.WriteLine( WriteFields( ss.ColNames ) );
                }

                for(int rowIndex = 0; rowIndex < ss.NumRows; ++rowIndex)
                {
                    if (this.HasRowNames)
                    {
                        sw.Write( WriteField( ss.RowNames[rowIndex] ) );
                        sw.Write( FullDelimiter );
                    }

                    IEnumerable row = ss.Rows[rowIndex];

                    if (converter == null)
                    {
                        sw.WriteLine( WriteFields( row ) );
                    }
                    else
                    {
                        sw.WriteLine( WriteFields( row.Cast<object>().Select( converter ) ) );
                    }
                }
            }
        }

        public void Write<T>( Spreadsheet<T> ss, string fileName, Func<T, string> converter = null )
        {                    
            Write( ss, fileName, converter );   
        }

        /// <summary>
        /// Reads a full spreadsheet.
        /// </summary>
        /// <typeparam name="T">Type of data to read</typeparam>
        /// <param name="text">Text data of spreadsheet</param>
        /// <param name="converter">Conversion of values to <typeparamref name="T"/> (NULL uses the default, if available)</param>
        /// <returns>The spreadsheet</returns>
        public Spreadsheet<T> ReadText<T>( string text, Converter<string, T> converter = null )
        {
            using (MemoryStream ms = new MemoryStream( Encoding.UTF8.GetBytes( text ) ))
            {
                using (StreamReader sr = new StreamReader( ms ))
                {
                    return Spreadsheet<T>.InternalRead( "INTERNAL_TEXT_DATA", sr, this, converter ?? GetConverter<T>() );
                }
            }
        }

        /// <summary>
        /// Reads a full spreadsheet.
        /// </summary>
        /// <typeparam name="T">Type of data to read</typeparam>
        /// <param name="fileName">Filename of spreadsheet</param>
        /// <param name="converter">Conversion of values to <typeparamref name="T"/> (NULL uses the default, if available)</param>
        /// <returns>The spreadsheet</returns>
        public Spreadsheet<T> Read<T>( string fileName, Converter<string, T> converter = null)
        {
            using (StreamReader sr = new StreamReader( fileName ))
            {
                return Read( sr, fileName, converter );
            }
        }

        /// <summary>
        /// Reads a full spreadsheet.
        /// </summary>
        /// <typeparam name="T">Type of data to read</typeparam>
        /// <param name="fileName">Filename of spreadsheet</param>
        /// <param name="converter">Conversion of values to <typeparamref name="T"/> (NULL uses the default, if available)</param>
        /// <returns>The spreadsheet</returns>
        public Spreadsheet<T> Read<T>( StreamReader streamReader, string title, Converter<string, T> converter = null )
        {                           
            return Spreadsheet<T>.InternalRead( title, streamReader, this, converter ?? GetConverter<T>() );
        }

        /// <summary>
        /// Reads a full spreadsheet of values.
        /// </summary>
        /// <param name="fileName">Filename of spreadsheet</param>                                                                
        /// <returns>The spreadsheet</returns>
        public Spreadsheet<Cell> Read( string fileName )
        {
            return Read<Cell>( fileName );
        }

        /// <summary>
        /// Writes a set of fields into a delimited string.
        /// Like <see cref="string.Join"/> but with escapement.
        /// </summary>            
        public string WriteFields( IEnumerable fields)
        {                                                
            StringBuilder result = new StringBuilder();

            if (fields == null)
            {
                return string.Empty;
            }

            foreach (object xxx in fields)
            {                                  
                string s = Convert.ToString( xxx );

                if (result.Length != 0)
                {
                    result.Append( FullDelimiter );
                }

                result.Append( WriteField(s) );
            }

            return result.ToString();
        }

        public string WriteField(string s)
        {
            if (s == null)
            {
                return string.Empty;
            }

            if (s.Contains( this.Delimiter ) || s.Contains( this.OpenQuote ) || s.Contains( this.CloseQuote ) || s.StartsWith( " " ) || s.EndsWith( " " ))
            {
                return "\""
                     + s.Replace( this.CloseQuote.ToString(), this.CloseQuote.ToString() + this.CloseQuote )
                     + "\"";
            }
            else
            {
                return s;
            }
        }

        /// <summary>
        /// Reads a set of fields from a delimited string.
        /// Like <see cref="string.Split"/> but with escapement.
        /// </summary>                                          
        public string[] ReadFields( string text )
        {                                     
            List<string> result = new List<string>();
            this.ParseFields( result, text );
            return result.ToArray();
        }

        /// <summary>
        /// Similar to <see cref="ReadFields"/> but avoid unnecessary parsing when
        /// only the number of fields is required.
        /// </summary>                         
        public int CountFields( string v )
        {   
            return this.ParseFields( null, v );
        }

        private int ParseFields( List<string> result, string text )
        {
            const int START = 0; // initial whitespace
            const int TEXT = 1; // text or...
            const int IN_QUOTES = 2; // ...quoted text
            const int END_QUOTES = 3; // after quotes (no more text expected)

            int fieldStart = 0;
            int startBeforeQuotes = 0;
            int fieldEnd = 0;
            int stage = START;
            int numberOfFields = 0;
            bool needToRemoveDoubleQuotes = false;

            //Debug.WriteLine("------------------------");

            for (int n = 0; n <= text.Length; n++)
            {
                char c;

                if (n == text.Length)
                {
                    c = this.Delimiter;
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

                if (c == this.Delimiter)
                {
                    if (stage != IN_QUOTES)
                    {
                        //Debug.Write( "end field " );
                        bool trimEnd = true;

                        if (stage != END_QUOTES)
                        {
                            fieldEnd = n;
                            trimEnd = false;
                        }

                        if (result != null)
                        {
                            string field = text.Substring( fieldStart, fieldEnd - fieldStart );

                            if (needToRemoveDoubleQuotes)
                            {
                                // TODO: Marginally inefficient
                                field = field.Replace( this.CloseQuote.ToString() + this.CloseQuote, this.CloseQuote.ToString() );
                            }

                            if (trimEnd)
                            {
                                field = field.TrimEnd(); // TODO: Marginally inefficient
                            }

                            result.Add( field );
                        }

                        fieldStart = n + 1;
                        ++numberOfFields;
                        stage = START;
                    }
                }
                else if (c == this.OpenQuote && stage == START)
                {
                    //Debug.Write( "start quote " );

                    startBeforeQuotes = fieldStart;
                    fieldStart = n + 1;
                    stage = IN_QUOTES;
                }
                else if (c == this.CloseQuote)
                {
                    if (stage == IN_QUOTES)
                    {
                        if (n != text.Length - 1 && text[n + 1] == this.CloseQuote)
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
                else if (!char.IsWhiteSpace( c ))
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

    public interface ISpreadsheet
    {
        string[] ColNames { get; }
        string[] RowNames { get; }
        IReadOnlyList<IEnumerable> Rows { get; }

        int NumRows { get; }
    }

    /// <summary>
    /// Represents a data matrix read from a file.
    /// </summary>
    /// <typeparam name="TCell">Type of data</typeparam>
    public class Spreadsheet<TCell> : ISpreadsheet
    {
        public string Title;
        public string[] RowNames => _rowNames;
        private readonly string[] _rowNames;

        public string[] ColNames => _colNames;
        private readonly string[] _colNames;

        /// <summary>
        /// Data (rows, cols)
        /// </summary>
        public readonly TCell[,] Data;

        public int NumRows => RowNames.Length;
        public int NumCols => ColNames.Length;

        /// <summary>
        /// CONSTRUCTOR
        /// PRIVATE, see <see cref="SpreadsheetReader.Read(string)"/>
        /// </summary>
        private Spreadsheet( string title, string[] rowNames, string[] colNames, TCell[,] data, int numRows, int numCols )
        {
            this.Title = title;
            _rowNames = rowNames;
            _colNames = colNames;
            this.Data = data;
        }

        public Spreadsheet( int nrow, int ncol )
        {
            Title = "Untitled spreadsheet of " + nrow + " rows and " + ncol + " columns";
            _rowNames = new string[nrow];
            _colNames = new string[ncol];
            Data = new TCell[nrow, ncol];
        }

        /// <summary>
        /// CONSTRUCTOR
        /// PRIVATE, see <see cref="Subset"/>
        /// </summary>          
        private Spreadsheet( Spreadsheet<TCell> origin, int[] rows, int[] cols )
        {
            if (rows == null)
            {
                rows = Enumerable.Range( 0, origin.NumRows ).ToArray();
            }

            if (cols == null)
            {
                cols = Enumerable.Range( 0, origin.NumCols ).ToArray();
            }

            _rowNames = origin.RowNames.Extract( rows );
            _colNames = origin.ColNames.Extract( cols );

            Data = new TCell[NumRows, NumCols];

            for (int row = 0; row < NumRows; row++)
            {
                for (int col = 0; col < NumCols; col++)
                {
                    int oRow = rows[row];
                    int oCol = cols[col];

                    Data[row, col] = origin[oRow, oCol];
                }
            }
        }

        /// <summary>
        /// Creates a subset of the current sheet.
        /// </summary>
        /// <param name="rows">Rows to use in the subset, or NULL for all.</param>
        /// <param name="cols">Columns to use in the subset, or NULL for all.</param>
        /// <returns>A copy of the subset of the current spreadsheet</returns>
        public Spreadsheet<TCell> Subset( int[] rows, int[] cols )
        {
            return new Spreadsheet<TCell>( this, rows, cols );
        }

        public TCell this[ int row, int col ]
        {
            get { return Data[row, col]; }
            set { Data[row, col] = value; }
        }

        internal static Spreadsheet<TCell> InternalRead( string title, StreamReader sr, SpreadsheetReader reader, Converter<string, TCell> converter )
        {
            int NumRows = 0;
            int NumCols = 0;
            TCell[,] Data;
            List<TCell[]> DataNs;
            string[] RowNames;
            List<string> RowNamesNs;
            string[] ColNames;
            bool seekable = sr.BaseStream.CanSeek;

            if (seekable)
            {
                // INITIAL READ TO GET SIZE OF DATA   
                if (reader.HasColNames)
                {
                    sr.ReadLine();
                }

                int fields = reader.CountFields( sr.ReadLine() );

                NumRows++;

                NumCols = reader.HasRowNames ? fields - 1 : fields;

                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();

                    if (line.Length == 0)
                    {
                        break;
                    }

                    NumRows++;
                }

                Data = new TCell[NumRows, NumCols];
                RowNames = new string[NumRows];
                RowNamesNs = null;
                DataNs = null;

                sr.BaseStream.Seek( 0, SeekOrigin.Begin );
                sr.DiscardBufferedData();
            }
            else
            {
                Data = null;
                RowNames = null;
                RowNamesNs = new List<string>();
                DataNs = new List<TCell[]>();
                NumRows = -1;
                NumCols = -1;
            }

            // READ OR CREATE COLUMN NAMES
            if (reader.HasColNames)
            {
                // First row name is blank
                string[] colNameData = reader.ReadFields( sr.ReadLine() );

                if (reader.HasRowNames)
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
                string[] lineData = reader.ReadFields( sr.ReadLine() );

                if (lineData.Length == 1 && lineData[0] == "")
                {
                    break;
                }

                int dataCol;

                if (reader.HasRowNames)
                {
                    dataCol = 1;

                    WriteRowName( RowNames, RowNamesNs, rowIndex, lineData[0] );                                    
                }
                else
                {
                    dataCol = 0;
                    WriteRowName( RowNames, RowNamesNs, rowIndex, "R" + rowIndex );
                }

                int colIndex = 0;

                if (Data != null)
                {
                    for (int c = dataCol; c < lineData.Length; c++)
                    {
                        try
                        {
                            Data[rowIndex, colIndex] = converter( lineData[c] );
                        }
                        catch (Exception ex)
                        {
                            throw new FormatException( $"Cannot parse the cell in row {{{rowIndex}}}, column {{{colIndex}}}, file {{{title}}}. " + ex.Message, ex );

                        }
                        colIndex++;
                    }
                }
                else
                {
                    TCell[] temp = new TCell[lineData.Length - dataCol];

                    for (int c = dataCol; c < lineData.Length; c++)
                    {
                        try
                        {
                            temp[colIndex] = converter( lineData[c] );
                        }
                        catch (Exception ex)
                        {
                            throw new FormatException( $"Cannot parse the cell in row {{{rowIndex}}}, column {{{colIndex}}}, file {{{title}}}. " + ex.Message, ex );
                        }

                        colIndex++;
                    }

                    DataNs.Add( temp );
                } 

                Assert( !seekable || colIndex == NumCols, $"Row {rowIndex} of the CSV file (filename = \"{title}\") has {colIndex} columns but at least one other row has {NumCols} columns. Check the CSV file for errors." );

                if (reader.Progress != null)
                {
                    if (!reader.Progress( rowIndex, NumRows ))
                    {
                        return null;
                    }
                }

                rowIndex++;
            }

            Assert( !seekable || rowIndex == NumRows, "Did not load all data for the CSV file \"" + title + "\". Check the CSV file for errors." );

            if (!seekable)
            {
                RowNames = RowNamesNs.ToArray();
                Data = ArrayHelper.Flatten( DataNs );        
            }

            return new Spreadsheet<TCell>( title, RowNames, ColNames, Data, Data.GetLength( 0 ), Data.GetLength( 1 ) );
        }

        private static void WriteRowName( string[] rowNames, List<string> rowNamesNs, int rowIndex, string v )
        {
            if (rowNames != null)
            {
                rowNames[rowIndex] = v;
            }
            else
            {
                rowNamesNs.Add( v );
            }
        }        

        /// <summary>
        /// Gets the index of the column with any of the specified title(s).
        /// </summary>
        /// <param name="colTitles">One or more titles</param>
        public int FindColumn( params string[] colTitles )
        {
            return InternalFind( colTitles, ColNames, typeof( Column ), true ); 
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
            return InternalFind( colTitles, ColNames, typeof( Column ), false );
        }

        private int InternalFind( string[] tries, IReadOnlyList< string> exists, Type method, bool throwOnError )
        {
            if (tries.Length == 0)
            {
                return -1;
            }

            int result = -1;

            foreach (string colTitle in tries)
            {
                int n = -1;

                for (int m = 0; m < exists.Count; m++)
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

        private void RaiseError( string[] tried, IReadOnlyList<string> exists, Type t, string problem )
        {
            string method = t.Name;
            throw new KeyNotFoundException( "Expected to find a " +method.ToUpper() + " with any of the titles {\"" + string.Join( "\", \"", tried ) + "\"} in the \"" + Title + "\" data but there are "+problem.ToUpper()+" matching " + method + "s in the array {\"" + string.Join( "\", \"", exists ) + "\"}. The names are not case sensitive. Check the CSV file for errors and make sure the settings are correct." );
        }

        public int TryFindRow( params string[] rowTitles )
        {
            return InternalFind( rowTitles, RowNames, typeof( Row ), false );
        }

        public int FindRow( params string[] rowTitles )
        {
            return InternalFind( rowTitles, RowNames, typeof( Row ), true );
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
        public TCell[] CopyColumn( int colIndex )
        {
            TCell[] result = new TCell[NumRows];

            for (int row = 0; row < result.Length; row++)
            {
                result[row] = this[row, colIndex];
            }

            return result;
        }

        /// <summary>
        /// Copies a row into an array.
        /// </summary>                   
        public TCell[] CopyRow( int rowIndex )
        {
            TCell[] result = new TCell[NumCols];

            for (int col = 0; col < result.Length; col++)
            {
                result[col] = this[rowIndex, col];
            }

            return result;
        }

        public static Spreadsheet<TCell> FromCsv( string fileName )
        {
            return SpreadsheetReader.Default.Read<TCell>( fileName );
        }

        public void SaveCsv( string fileName )
        {
            SpreadsheetReader.Default.Write( this, fileName );
        }

        public RowCollection Rows => new RowCollection( this );

        IReadOnlyList<IEnumerable> ISpreadsheet.Rows => Rows;

        public ColumnCollection Columns => new ColumnCollection( this );

        public abstract class HeaderCollection<THeader> : IReadOnlyList<THeader>
        {
            protected readonly Spreadsheet<TCell> _spreadsheet;

            public HeaderCollection( Spreadsheet<TCell> spreadsheet )
            {
                this._spreadsheet = spreadsheet;
            }

            public abstract int Count { get; }

            public abstract THeader this[int index] { get; }

            public THeader this[params string[] possibleNames]
            {
                get
                {
                    return Find( possibleNames );
                }
            }

            public abstract IReadOnlyList<string> Names { get; }

            public IEnumerator<THeader> GetEnumerator()
            {
                for (int n = 0; n < Count; n++)
                {
                    yield return this[n];
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public THeader Find( params string[] possibleNames )
            {
                return this[FindIndex( possibleNames )];
            }

            public THeader TryFind( params string[] possibleNames )
            {
                int index = TryFindIndex( possibleNames );

                if (index != -1)
                {
                    return this[index];
                }
                else
                {
                    return default( THeader );
                }
            }

            public bool Contains( params string[] possibleNames )
            {
                return TryFindIndex( possibleNames ) != -1;
            }

            public int FindIndex( params string[] possibleNames )
            {
                return _spreadsheet.InternalFind(possibleNames, Names, typeof( THeader ), true );
            }

            public int TryFindIndex( params string[] possibleNames )
            {
                return _spreadsheet.InternalFind( possibleNames, Names, typeof( THeader ), false );
            }
        }

        public class ColumnCollection : HeaderCollection<Column>
        {     
            public ColumnCollection( Spreadsheet<TCell> spreadsheet ) 
                :base( spreadsheet )
            {
                // NA
            }

            public override int Count => _spreadsheet.NumCols;

            public override IReadOnlyList<string> Names => _spreadsheet.ColNames;

            public override Column this[int columnIndex] => new Column( _spreadsheet, columnIndex );   
        }

        public class RowCollection : HeaderCollection<Row>
        {                             
            public RowCollection( Spreadsheet<TCell> spreadsheet )
                : base( spreadsheet )
            {
                // NA
            }

            public override int Count => _spreadsheet.NumRows;

            public override IReadOnlyList<string> Names => _spreadsheet.RowNames;

            public override Row this[int rowIndex]=> new Row( _spreadsheet, rowIndex );       
        }    

        public class Row : IReadOnlyList<TCell>
        {
            public readonly int Index;
            public readonly Spreadsheet<TCell> Spreadsheet;

            public Row( Spreadsheet<TCell> spreadsheet, int rowIndex )
            {
                this.Spreadsheet = spreadsheet;
                this.Index = rowIndex;
            }

            public TCell this[int column]=> Spreadsheet[Index, column];

            public TCell this[Column column] => Spreadsheet[Index, column.Index];

            public TCell this[string column] => Spreadsheet[Index, Spreadsheet.FindColumn(column)];

            public string Name
            {
                get
                {
                    return Spreadsheet.RowNames[Index];
                }
                set
                {
                    Spreadsheet.RowNames[Index] = value;
                }
            }

            public int Count => Spreadsheet.NumCols;

            public IEnumerator<TCell> GetEnumerator()
            {
                for (int col = 0; col < Spreadsheet.NumCols; col++)
                {
                    yield return Spreadsheet[Index, col];
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public class Column : IReadOnlyList<TCell>
        {
            public readonly int Index;
            private Spreadsheet<TCell> _spreadsheet;

            public Column( Spreadsheet<TCell> spreadsheet, int column )
            {
                this._spreadsheet = spreadsheet;
                this.Index = column;
            }

            public TCell this[int row] => _spreadsheet[row, Index];

            public TCell this[Row row] => _spreadsheet[row.Index, Index];

            public string Name
            {
                get
                {
                    return _spreadsheet.ColNames[Index];
                }
                set
                {
                    _spreadsheet.ColNames[Index] = value;
                }
            }

            public int Count => _spreadsheet.NumRows;

            public IEnumerator<TCell> GetEnumerator()
            {
                for (int row = 0; row < _spreadsheet.NumRows; row++)
                {
                    yield return _spreadsheet[row, Index];
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }      
    }

    public struct Cell
    {
        private string _string;

        public Cell( string @string)
        {
            _string = @string;
        }   

        public string String
        {
            get
            {
                return _string;
            }
            set
            {
                _string = value;
            }
        }

        public int Int32
        {
            get
            {
                return int.Parse( String );
            }
            set
            {
                String = value.ToString();
            }
        }

        public double Double
        {
            get
            {
                return double.Parse( String );
            }
            set
            {
                String = value.ToString();
            }
        }

        public static implicit operator string( Cell cell )
        {
            return cell.String;
        }

        public static implicit operator int( Cell cell )
        {
            return cell.Int32;
        }

        public static implicit operator double( Cell cell )
        {
            return cell.Double;
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
