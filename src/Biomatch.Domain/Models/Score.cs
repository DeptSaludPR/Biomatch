namespace Biomatch.Domain.Models;

public static class Score
{
  private static double SingleFieldScoreStepMode(int distance, int threshold = 2, double step = 0.3)
  {
    // Experimental version for now, assigns 1 if the distance is 0, and lowers in increments to
    // 0.7
    double singleFieldScore;

    // If the distance > threshold, assign 0
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
    // else, return 1-(step/threshold) * dist.
    // This results in a higher individual score the closer the distance is to 0
    // culminating at 1-step at the threshold
    else
    {
      singleFieldScore = 1 - step * ((double)distance / threshold);
    }

    return singleFieldScore;
  }

  public static double GetScore(int distance, double weight, int threshold)
  {
    const double step = 0.3;
    // Experimental version for now, assigns 1 if the distance is 0, and lowers in increments to
    // 0.7
    double singleFieldScore;

    // If the distance > threshold, assign 0
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
    // else, return 1-(step/threshold) * dist.
    // This results in a higher individual score the closer the distance is to 0
    // culminating at 1-step at the threshold
    else
    {
      singleFieldScore = 1 - step * ((double)distance / threshold);
    }

    return singleFieldScore * weight;
  }

  public static double CalculateFinalScore(
    int firstNameDistance,
    int middleNameDistance,
    int lastNameDistance,
    int secondLastNameDistance,
    int fullNameDistance,
    int birthDateDistance,
    int cityDistance,
    int phoneNumberDistance,
    int firstNameThreshold = 2,
    int middleNameThreshold = 1,
    int lastNameThreshold = 2,
    int fullNameThreshold = 5,
    int secondLastNameThreshold = 2,
    int birthDateThreshold = 1,
    int cityThreshold = 2,
    int phoneNumberThreshold = 1,
    double firstNameWeight = 0.18,
    double middleNameWeight = 0.1,
    double lastNameWeight = 0.17,
    double secondLastNameWeight = 0.17,
    double fullNameWeight = 0.62,
    double birthDateWeight = 0.20,
    double cityWeight = 0.08,
    double phoneNumberWeight = 0.1
  )
  {
    // Get the individual field score distances
    var firstNameSingleScore = SingleFieldScoreStepMode(firstNameDistance, firstNameThreshold);
    var middleNameSingleScore = SingleFieldScoreStepMode(middleNameDistance, middleNameThreshold);
    var lastNameSingleScore = SingleFieldScoreStepMode(lastNameDistance, lastNameThreshold);
    var secondLastNameSingleScore = SingleFieldScoreStepMode(
      secondLastNameDistance,
      secondLastNameThreshold
    );
    var fullNameSingleScore = SingleFieldScoreStepMode(fullNameDistance, fullNameThreshold);
    var birthDateSingleScore = SingleFieldScoreStepMode(birthDateDistance, birthDateThreshold);
    var citySingleScore = SingleFieldScoreStepMode(cityDistance, cityThreshold);
    var phoneNumberSingleScore = SingleFieldScoreStepMode(
      phoneNumberDistance,
      phoneNumberThreshold
    );

    var separateNameScore =
      firstNameSingleScore * firstNameWeight
      + middleNameSingleScore * middleNameWeight
      + lastNameSingleScore * lastNameWeight
      + secondLastNameSingleScore * secondLastNameWeight;
    var fullNameScore = fullNameSingleScore * fullNameWeight;

    var nameScore = separateNameScore > fullNameScore ? separateNameScore : fullNameScore;

    // Then compute the weighted average
    var totalScore = nameScore;
    totalScore += birthDateWeight * birthDateSingleScore;
    totalScore += cityWeight * citySingleScore;
    totalScore += phoneNumberWeight * phoneNumberSingleScore;
    return totalScore;
  }
}
