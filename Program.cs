
namespace MatchingEngine
{
    class Program
    {
        private static void Main(string[] args)
        {
            Run.Run_TwoFileComparison_v2("ACTIVE_CASE_API_2021_07_23.csv", "CASE_API_2021_07_23.csv", 
            "2021_08_11_test_run_for_duplicates",false,123,125,true,1,100,true,false,0.7);

            

        }
    }
}
