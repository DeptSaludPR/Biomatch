using System;

namespace CSV_Example_Code
{
    public class DistanceVector : PatientInfoRecord
    {
        private int _firstNameDist=0;
        private int _middleNameDist = 0;
        private int _lastNameDist =0;
        private int _secondLastNameDist=0;
        private int _birthDateDist=0;
        private int _cityDist=0;
        private int _phoneNumberDist=0;

        public int FirstNameDist { get => _firstNameDist; set => _firstNameDist = value; }
        public int MiddleNameDist { get => _middleNameDist; set => _middleNameDist = value; }
        public int LastNameDist { get => _lastNameDist; set => _lastNameDist = value; }
        public int SecondLastNameDist { get => _secondLastNameDist; set => _secondLastNameDist = value; }
        public int BirthDateDist { get => _birthDateDist; set => _birthDateDist = value; }
        public int CityDist { get => _cityDist; set => _cityDist = value; }
        public int PhoneNumberDist { get => _phoneNumberDist; set => _phoneNumberDist = value; }

        public DistanceVector()
        {
            FirstNameDist = 0;
            MiddleNameDist = 0;
            LastNameDist = 0;
            SecondLastNameDist = 0;
            BirthDateDist=0;
            CityDist=0;
            PhoneNumberDist=0;
        } 
        public DistanceVector(int First_Name = 0, int MiddleName=0, int LastName=0, int SLastName=0, int bday=0, int city=0, int pn=0)
        {
            FirstNameDist = First_Name;
            MiddleNameDist = MiddleName;
            LastNameDist=LastName;
            SecondLastNameDist=SLastName;
            BirthDateDist=bday;
            CityDist=city;
            PhoneNumberDist=pn;
        }
        public void displayVectorInfo()
        {
            Console.WriteLine($"First Name: {this.FirstNameDist} MiddleName: {this.MiddleNameDist} Lastname: {this.LastNameDist} SecondLastname: {this.SecondLastNameDist} BirthDate: {BirthDateDist} City: {CityDist} PhoneNumber: {PhoneNumberDist}");
        } 
        public string getVectorInfo()
        {
            return ($"First Name: {this.FirstNameDist} MiddleName: {this.MiddleNameDist} Lastname: {this.LastNameDist} SecondLastname: {this.SecondLastNameDist} BirthDate: {BirthDateDist} City: {CityDist} PhoneNumber: {PhoneNumberDist}\n");
        }
        void setFirstNameDist(int First_Name)
        {
            FirstNameDist=First_Name;
        }
        public int getFirstNameDist()
        {
            return(FirstNameDist);
        }
        void setMiddleNameDist(int MiddleName)
        {
            MiddleNameDist=MiddleName;
        }

        public int getMiddleNameDist()
        {
            return MiddleNameDist;
        }

        void setLastNameDist(int LastName)
        {
            LastNameDist = LastName;
        }

        public int getLastNameDist()
        {
            return LastNameDist;
        }

        void setSecondLastNameDist(int SecondLastName)
        {
            SecondLastNameDist = SecondLastName;
        }
        
        public int getSecondLastNameDist( )
        {
            return SecondLastNameDist; 
        }

        public void setBirthDateDist(int BirthDate )
        {
            BirthDateDist=BirthDate;
        }

        public int getBirthDateDist( )
        {
            return BirthDateDist;
        }

        public void setCityDist( int city)
        {
            CityDist=city;
        }

        public int getCityDist( )
        {
            return CityDist;
        }

        public void setPhoneNumberDist( int pn)
        {
            PhoneNumberDist=pn;
        }

        public int getPhoneNumberDist ( )
        {
            return PhoneNumberDist;
        }

        public string csvFormatString()
        {  
            
            string str=$"[{this.getFirstNameDist()} {this.getMiddleNameDist()} {this.getLastNameDist()} ";
            str+=$"{this.getSecondLastNameDist()} {this.getBirthDateDist()} {this.getCityDist()} {this.getPhoneNumberDist()}]";
            return str;
        }
        public DistanceVector calculateDistance(PatientInfoRecord A, PatientInfoRecord B)
        {
            
            DistanceVector result = new DistanceVector();  
            result.setFirstNameDist(StringDistance.GeneralDemographicFieldDistance(A.FirstName, B.FirstName));
            result.setMiddleNameDist(StringDistance.GeneralDemographicFieldDistance(A.MiddleName, B.MiddleName));
            result.setLastNameDist(StringDistance.GeneralDemographicFieldDistance(A.LastName, B.LastName));
            result.setSecondLastNameDist(StringDistance.GeneralDemographicFieldDistance(A.SecondLastName, B.SecondLastName));
            result.setBirthDateDist(StringDistance.GeneralDemographicFieldDistance(A.BirthDate, B.BirthDate));
            result.setCityDist(StringDistance.GeneralDemographicFieldDistance(A.City, B.City));
            result.setPhoneNumberDist(StringDistance.GeneralDemographicFieldDistance(A.PhoneNumber, B.PhoneNumber));
            return result;
        }
    }


    public static class StringDistance
    {
        /// <summary>
        /// Compute the distance between two strings.
        /// </summary>
        public static int LevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            { return m; }
            if (m == 0)
            { return n; }
            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            { }
            for (int j = 0; j <= m; d[0, j] = j++)
            { }
            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }

        public static int LowMemoryLevenshteinDist(String sRow, String sCol)
        {
            int RowLen = sRow.Length;  // length of sRow
            int ColLen = sCol.Length;  // length of sCol
            int RowIdx;                // iterates through sRow
            int ColIdx;                // iterates through sCol
            char Row_i;                // ith character of sRow
            char Col_j;                // jth character of sCol
            int cost;                   // cost

            

            // Step 1

            if (RowLen == 0)
            {
                return ColLen;
            }

            if (ColLen == 0)
            {
                return RowLen;
            }

            /// Create the two vectors
            int[] v0 = new int[RowLen + 1];
            int[] v1 = new int[RowLen + 1];
            int[] vTmp;


            
            /// Step 2
            /// Initialize the first vector
            for (RowIdx = 1; RowIdx <= RowLen; RowIdx++)
            {
                v0[RowIdx] = RowIdx;
            }

            // Step 3

            /// Fore each column
            for (ColIdx = 1; ColIdx <= ColLen; ColIdx++)
            {
                /// Set the 0'th element to the column number
                v1[0] = ColIdx;

                Col_j = sCol[ColIdx - 1];


                // Step 4

                /// Fore each row
                for (RowIdx = 1; RowIdx <= RowLen; RowIdx++)
                {
                    Row_i = sRow[RowIdx - 1];


                    // Step 5

                    if (Row_i == Col_j)
                    {
                        cost = 0;
                    }
                    else
                    {
                        cost = 1;
                    }

                    // Step 6

                    /// Find minimum
                    int m_min = v0[RowIdx] + 1;
                    int b = v1[RowIdx - 1] + 1;
                    int c = v0[RowIdx - 1] + cost;

                    if (b < m_min)
                    {
                        m_min = b;
                    }
                    if (c < m_min)
                    {
                        m_min = c;
                    }

                    v1[RowIdx] = m_min;
                }

                /// Swap the vectors
                vTmp = v0;
                v0 = v1;
                v1 = vTmp;
            }
            return(v0[RowLen]);
        }

        public static int GeneralDemographicFieldDistance( string a, string b )
        {
            //check for empty values
            if(a=="" & b=="")
            {
                return 0;
            }
            else if(a=="" | b=="")
            {
                return(-1);
            }
            else
            {
                return(StringDistance.LowMemoryLevenshteinDist(a,b));
                
            }
        }

        public static int MiddleNameFieldDistance( string a, string b)
        {
            if( a.Length==1 | b.Length==1)
            {
                if (a.Substring(0,1)==b.Substring(0,1))
                {
                    return(0);
                }
                else
                {
                    return StringDistance.LowMemoryLevenshteinDist(a,b);
                }
                
            }
            else
                {
                    return StringDistance.LowMemoryLevenshteinDist(a,b);
                }
        }
    }
    public class Score
    {
        public double singleFieldScore_StepMode(int dist, int treshold=2, double step=0.3){
            //Experimental version for now, assigns 1 if the distance is 0, and lowers in increments to 
            //0.7 

            //If the distance > treshold, assign 0
            if(dist>treshold)
            {
                return(0.0);
            } 
            // else if the distance is -1 (special exception) return 0.5 
            // this corresponds to the case where one is empty and the other is not 
            else if(dist==-1)
            {
                return(0.5);
            }
            //else, retrun 1-(step/treshold)*dist. 
            //This results in a higher individual score the closer the distance is to 0 
            //culminating at 1-step at the treshold
            else
            {
                return(1-step*(dist/treshold));
            }
        }

        public double allFieldsScore_StepMode( DistanceVector d, 
        int fn_treshold=2, int mn_treshold=1, int ln_treshold=2, int sln_treshold=2, 
        int bday_treshold=1,int city_treshold=2, int pn_treshold=1,
        double fn_weight=0.18, double mn_weight=0.1, double ln_weight=0.18, double sln_weight=0.16,
        double bday_weight=0.20, double city_weight=0.08, double pn_weight=0.1)
        {
            double check_sum=fn_weight+mn_weight+ln_weight+sln_weight+bday_weight+city_weight+pn_weight;
            if(check_sum!=1.0){
                throw new Exception("sum of weights is not 1.0");
            }
            //get the individual field score distances
            double fn_d=singleFieldScore_StepMode(d.getFirstNameDist(), fn_treshold);
            double mn_d=singleFieldScore_StepMode(d.getMiddleNameDist(), mn_treshold);
            double ln_d=singleFieldScore_StepMode(d.getLastNameDist(), ln_treshold);
            double sln_d=singleFieldScore_StepMode(d.getSecondLastNameDist(), sln_treshold);
            double bday_d=singleFieldScore_StepMode(d.getBirthDateDist(), bday_treshold);
            double city_d=singleFieldScore_StepMode(d.getCityDist(), city_treshold);
            double pn_d=singleFieldScore_StepMode(d.getPhoneNumberDist(), pn_treshold);
            //Then compute the weighted average
            double total_score=0.0;

            total_score+=fn_weight*fn_d;
            total_score+=mn_weight*mn_d;
            total_score+=ln_weight*ln_d;
            total_score+=sln_weight*sln_d;
            total_score+=bday_weight*bday_d;
            total_score+=city_weight*city_d;
            total_score+=pn_weight*pn_d;
            return(total_score);
        }
    }


    public class Record 
    {
        private PatientInfoRecord _patientInfo= new PatientInfoRecord();
        private string _idType="unspecified";
        private Guid _RecordId=Guid.NewGuid();


        public PatientInfoRecord PatientInfo { get => _patientInfo; set => _patientInfo = value; }
        public string IdType { get => _idType; set => _idType = value; }
        public Guid RecordId { get => _RecordId; set => _RecordId = value; }

        public Record()
        {
            _patientInfo=new PatientInfoRecord();
            _idType="unspecifiedId";
            _RecordId=Guid.NewGuid();
        }
        public Record (PatientInfoRecord patientInfoRecord, string idtype)
        {
            _patientInfo=patientInfoRecord;
            _idType=idtype;
            _RecordId=Guid.NewGuid();
        }

        public Record ( PatientInfoRecord patientInfoRecord, string idtype ,string id)
        {
            string temp_id=id;

            _patientInfo=patientInfoRecord;
            _idType=idtype;
            try
            {
                _RecordId=new Guid(id);
            } catch
            {
                _RecordId=Guid.NewGuid();
                Console.WriteLine($"Guid problem with id: {temp_id}");
            }
            
        }
    }


    public class PotentialDuplicate
    {
        private Record _value;
        private Record _match;
        private DistanceVector _distance;
        private double _score;
        public Record Value { get => _value; set => _value = value; }
        public Record Match { get => _match; set => _match = value; }
        public DistanceVector Distance { get => _distance; set => _distance = value; }
        public double Score { get => _score; set => _score = value; }
        

        public string csvColumnNames( )
        {
            string col_names;
            col_names="Value.IdType,Value.ID,Match.IdType,Match.ID,Score,Distance";
            col_names+=",Value.FirstName,Value.MiddleName,Value.LastName,Value.SecondLastName,Value.BirthDate,Value.City,Velue.PhoneNumber";
            col_names+=",Match.FirstName,Match.MiddleName,Match.LastName,Match.SecondLastName,Match.BirthDate,Match.City,Match.PhoneNumber";
            return(col_names);
        }
        public string csvColumnNamesAsterisk( )
        {
            string col_names;
            col_names="Value.IdType,Value.ID,Match.IdType,Match.ID,Score,Distance";
            col_names+=",Value.PatientInfoAsterisk";
            col_names+=",Match.PatientInfoAterisk";


            return(col_names);
        }
        public string csvFormatString( )
        {
            string str;
            str=$"{Value.IdType},{Value.RecordId},{Match.IdType},{Match.RecordId},{Score},{Distance.csvFormatString()}";
            str+=$",{Value.PatientInfo.StringToCSVFormat()},{Match.PatientInfo.StringToCSVFormat()}";
            return(str);
        }
        public string csvFormatStringAsterisk( )
        {
            string str;
            str=$"{Value.IdType},{Value.RecordId},{Match.IdType},{Match.RecordId},{Score},{Distance.csvFormatString()}";
            str+=$",{Value.PatientInfo.StringToCSVFormatAsterisk()},{Match.PatientInfo.StringToCSVFormatAsterisk()}";
            return str;
            

        }
        public PotentialDuplicate()
        {
            _value=new Record();
            _match= new Record();
            _distance= new DistanceVector();
            _score =0.0;
        }
        public PotentialDuplicate(Record record1, Record record2, DistanceVector d, double score)
        {
            _value=record1;
            _match=record2;
            _distance=d;
            _score=score;
        }
    }
    public class determenistic : DistanceVector
    {
        
    }
}