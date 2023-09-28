namespace Biomatch.Domain.Models;

public static class Score
{
  private static double SingleFieldScoreStepMode(int distance, int threshold = 2, double step = 0.3)
  {
    //Experimental version for now, assigns 1 if the distance is 0, and lowers in increments to
    //0.7
    double singleFieldScore;

    //If the distance > threshold, assign 0
    if (distance > threshold)
    {
      singleFieldScore = 0.0;
    }
    // else if the distance is -1 (special exception) return 0.5
    // this corresponds to the case where one is empty and the other is not
    else if (distance == -1)
    {
      singleFieldScore = 0.5;
    }
    //else, return 1-(step/threshold)*dist.
    //This results in a higher individual score the closer the distance is to 0
    //culminating at 1-step at the threshold
    else
    {
      singleFieldScore = 1 - step * ((double)distance / threshold);
    }

    return singleFieldScore;
  }

  public static double CalculateFinalScore(
    ref DistanceVector d,
    int firstNameThreshold = 2,
    int middleNameThreshold = 1,
    int lastNameThreshold = 2,
    int secondLastNameThreshold = 2,
    int birthDateThreshold = 1,
    int cityThreshold = 2,
    int phoneNumberThreshold = 1,
    double firstNameWeight = 0.18,
    double middleNameWeight = 0.1,
    double lastNameWeight = 0.17,
    double secondLastNameWeight = 0.17,
    double birthDateWeight = 0.20,
    double cityWeight = 0.08,
    double phoneNumberWeight = 0.1
  )
  {
    // Get the individual field score distances
    var firstNameDistance = SingleFieldScoreStepMode(d.FirstNameDistance, firstNameThreshold);
    var middleNameDistance = SingleFieldScoreStepMode(d.MiddleNameDistance, middleNameThreshold);
    var lastNameDistance = SingleFieldScoreStepMode(d.LastNameDistance, lastNameThreshold);
    var secondLastNameDistance = SingleFieldScoreStepMode(
      d.SecondLastNameDistance,
      secondLastNameThreshold
    );
    var birthDateDistance = SingleFieldScoreStepMode(d.BirthDateDistance, birthDateThreshold);
    var cityDistance = SingleFieldScoreStepMode(d.CityDistance, cityThreshold);
    var phoneNumberDistance = SingleFieldScoreStepMode(d.PhoneNumberDistance, phoneNumberThreshold);

    // Then compute the weighted average
    var totalScore = firstNameWeight * firstNameDistance;
    totalScore += middleNameWeight * middleNameDistance;
    totalScore += lastNameWeight * lastNameDistance;
    totalScore += secondLastNameWeight * secondLastNameDistance;
    totalScore += birthDateWeight * birthDateDistance;
    totalScore += cityWeight * cityDistance;
    totalScore += phoneNumberWeight * phoneNumberDistance;
    return totalScore;
  }
}
