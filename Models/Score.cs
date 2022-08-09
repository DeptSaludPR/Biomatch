using System;

namespace MatchingEngine.Models;

public class Score
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
            singleFieldScore = 1 - step * ((double)dist / threshold);
        }

        return singleFieldScore;
    }

    public static double allFieldsScore_StepMode(DistanceVector d,
        int fn_treshold = 2, int mn_treshold = 1, int ln_treshold = 2, int sln_treshold = 2,
        int bday_treshold = 1, int city_treshold = 2, int pn_treshold = 1,
        double fn_weight = 0.18, double mn_weight = 0.1, double ln_weight = 0.17, double sln_weight = 0.17,
        double birthDateWeight = 0.20, double cityWeight = 0.08, double pn_weight = 0.1)
    {
        var checkSum = fn_weight + mn_weight + ln_weight + sln_weight + birthDateWeight + cityWeight + pn_weight;
        if (Math.Abs(checkSum - 1.0) != 0)
        {
            throw new Exception("sum of weights is not 1.0");
        }

        //get the individual field score distances
        var firstNameDistance = singleFieldScore_StepMode(d.FirstNameDistance, fn_treshold);
        var middleNameDistance = singleFieldScore_StepMode(d.MiddleNameDistance, mn_treshold);
        var lastNameDistance = singleFieldScore_StepMode(d.LastNameDistance, ln_treshold);
        var secondLastNameDistance = singleFieldScore_StepMode(d.SecondLastNameDistance, sln_treshold);
        var birthDateDistance = singleFieldScore_StepMode(d.BirthDateDistance, bday_treshold);
        var cityDistance = singleFieldScore_StepMode(d.CityDistance, city_treshold);
        var phoneNumberDistance = singleFieldScore_StepMode(d.PhoneNumberDistance, pn_treshold);
        //Then compute the weighted average
        var totalScore = 0.0;

        totalScore += fn_weight * firstNameDistance;
        totalScore += mn_weight * middleNameDistance;
        totalScore += ln_weight * lastNameDistance;
        totalScore += sln_weight * secondLastNameDistance;
        totalScore += birthDateWeight * birthDateDistance;
        totalScore += cityWeight * cityDistance;
        totalScore += pn_weight * phoneNumberDistance;
        return totalScore;
    }
}