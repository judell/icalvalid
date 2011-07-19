using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator
{
    public class ScoreCalculator
    {
        public delegate void ValidationResultInfoDelegate(IValidationResultInfo info);

        static private void Traverse(IValidationResultCollection results, ValidationResultInfoDelegate d)
        {
            foreach (IValidationResultInfo info in results.Details)
                d.Invoke(info);
        }

        static private double Curve(double i, double max)
        {
            return Math.Min(max, Math.Sqrt(i) * 5);
        }

        static private double ScoreDuplicates(double max, Dictionary<string, int> map)
        {
            double score = 0.0;
            foreach (KeyValuePair<string, int> kvp in map)
            {
                if (kvp.Value > 1)
                    score += Curve(kvp.Value - 1, max);
            }
            return score;
        }

        static public double CalculateScore(IValidationResultCollection results)
        {
            List<string> foundErrors = new List<string>();
            Dictionary<ValidationResultInfoType, Dictionary<string, int>> errorMap = new Dictionary<ValidationResultInfoType, Dictionary<string, int>>();

            errorMap[ValidationResultInfoType.Warning] = new Dictionary<string,int>();
            errorMap[ValidationResultInfoType.Error] = new Dictionary<string, int>();
            errorMap[ValidationResultInfoType.Fatal] = new Dictionary<string, int>();

            Traverse(results,
                delegate(IValidationResultInfo info)
                {
                    if (!errorMap[info.Type].ContainsKey(info.Name))
                        errorMap[info.Type][info.Name] = 0;
                    errorMap[info.Type][info.Name]++;
                }
            );

            double score =
                Math.Max(0,
                    100 -
                    (errorMap[ValidationResultInfoType.Fatal].Count * 25) -
                    ScoreDuplicates(50, errorMap[ValidationResultInfoType.Fatal]) -
                    (errorMap[ValidationResultInfoType.Error].Count * 5) -
                    ScoreDuplicates(30, errorMap[ValidationResultInfoType.Error]) -
                    (errorMap[ValidationResultInfoType.Warning].Count) -
                    ScoreDuplicates(5, errorMap[ValidationResultInfoType.Warning]));

            return score;
        }
    }
}
