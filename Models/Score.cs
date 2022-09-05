namespace MatchingEngine.Models;

public static class Score
{
    private static double singleFieldScore_StepMode(int dist, int threshold = 2, double step = 0.3)
    {
        //Experimental version for now, assigns 1 if the distance is 0, and lowers in increments to 
        //0.7 

        double singleFieldScore;

        //If the distance > threshold, assign 0
        if (dist > threshold)
        {
            singleFieldScore = 0.0;
        }
        // else if the distance is -1 (special exception) return 0.5 
        // this corresponds to the case where one is empty and the other is not 
        else if (dist == -1)
        {
            singleFieldScore = 0.5;
        }
        //else, return 1-(step/threshold)*dist. 
        //This results in a higher individual score the closer the distance is to 0 
        //culminating at 1-step at the threshold
        else
        {
            singleFieldScore = 1 - step * ((double) dist / threshold);
        }

        return singleFieldScore;
    }

    public static double allFieldsScore_StepMode(ref DistanceVector d,
        int fnThreshold = 2, int mnThreshold = 1, int lnThreshold = 2, int slnThreshold = 2,
        int birthDateThreshold = 1, int cityThreshold = 2, int pnThreshold = 1,
        double fnWeight = 0.18, double mnWeight = 0.1, double lnWeight = 0.17, double slnWeight = 0.17,
        double birthDateWeight = 0.20, double cityWeight = 0.08, double pnWeight = 0.1)
    {
        //get the individual field score distances
        var firstNameDistance = singleFieldScore_StepMode(d.FirstNameDistance, fnThreshold);
        var middleNameDistance = singleFieldScore_StepMode(d.MiddleNameDistance, mnThreshold);
        var lastNameDistance = singleFieldScore_StepMode(d.LastNameDistance, lnThreshold);
        var secondLastNameDistance = singleFieldScore_StepMode(d.SecondLastNameDistance, slnThreshold);
        var birthDateDistance = singleFieldScore_StepMode(d.BirthDateDistance, birthDateThreshold);
        var cityDistance = singleFieldScore_StepMode(d.CityDistance, cityThreshold);
        var phoneNumberDistance = singleFieldScore_StepMode(d.PhoneNumberDistance, pnThreshold);
        //Then compute the weighted average
        var totalScore = fnWeight * firstNameDistance;
        totalScore += mnWeight * middleNameDistance;
        totalScore += lnWeight * lastNameDistance;
        totalScore += slnWeight * secondLastNameDistance;
        totalScore += birthDateWeight * birthDateDistance;
        totalScore += cityWeight * cityDistance;
        totalScore += pnWeight * phoneNumberDistance;
        return totalScore;
    }
}