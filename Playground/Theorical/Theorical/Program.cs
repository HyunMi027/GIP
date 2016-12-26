using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Theorical
{
    public static class Program
    {
        public static AcidBaseMixture Mixture = new AcidBaseMixture("H2SO4", new List<DissociationConstant>() {new DissociationConstant(double.PositiveInfinity, 0), new DissociationConstant(1.2, -2)}, 0.1, 0,
                                                                    "NaOH", new List<DissociationConstant>() {new DissociationConstant(double.PositiveInfinity, 0)}, 0.1, 1);

        public static List<Point> Results = new List<Point>(); 

        public static void Main(string[] args)
        {
            while (Mixture.Acid.Volume < 2)
            {
                Results.Add(new Point(Mixture.Acid.Volume, Mixture.pH));
                Mixture.Acid.Volume += 0.000001;
            }

            Debug.WriteLine(JsonConvert.SerializeObject(Results));
            Console.ReadKey();
        }

        public class Point
        {
            public double Volume { get; private set; }
            public double pH { get; private set; }

            public Point(double volume, double pH)
            {
                this.Volume = volume;
                this.pH = pH;
            }
        }

        public class AcidBaseMixture
        {
            public Substance Acid { get; set; }
            public Substance Base { get; set; }

            public double TotalVolume => this.Acid.Volume + this.Base.Volume;

            public double NetHIonConcentration => this.Acid.Dissociate().Item1*this.Acid.Volume/this.TotalVolume - this.Base.Dissociate().Item1*this.Base.Volume/this.TotalVolume;

            public double pH
            {
                get
                {
                    if (Math.Abs(this.NetHIonConcentration) <= 0.0000001)
                    {
                        return 7;
                    }

                    double res;
                    if (this.NetHIonConcentration < 0)
                    {
                        res = 14 + Math.Log10(Math.Abs(this.NetHIonConcentration));
                    }
                    else
                    {
                        res = -Math.Log10(this.NetHIonConcentration);
                    }

                    if (res > 14)
                    {
                        res = 14;
                    }
                    else if (res < 0)
                    {
                        res = 0;
                    }

                    return res;
                }
            }

            public AcidBaseMixture(Substance acid, Substance base_)
            {
                this.Acid = acid;
                this.Base = base_;
            }

            public AcidBaseMixture(string acidFormula, List<DissociationConstant> acidDissociationConstants, double acidConcentration, double acidVolume,
                                   string baseFormula, List<DissociationConstant> baseDissociationConstants, double baseConcentration, double baseVolume)
            {
                this.Acid = new Substance(acidFormula, acidDissociationConstants, true, acidConcentration, acidVolume);
                this.Base = new Substance(baseFormula, baseDissociationConstants, false, baseConcentration, baseVolume);
            }
        }

        [Serializable]
        public class Substance
        {
            [JsonProperty("BaseFormula")]
            public string Formula { get; set; }
            [JsonProperty("DissociationConstants")] // Also indicates amount of dissociatable ions
            public List<DissociationConstant> DissociationConstants { get; set; }
            [JsonProperty("IsAcid")]
            public bool IsAcid { get; set; }

            [JsonIgnore]
            public double Concentration { get; set; }
            [JsonIgnore]
            public double Volume { get; set; }

            public double Moles => this.Volume * this.Concentration;

            public Substance(string formula, List<DissociationConstant> dissociationConstants, bool isacid, double concentration, double volume = 0)
            {
                this.Formula = formula;
                this.DissociationConstants = dissociationConstants;
                this.IsAcid = isacid;
                this.Concentration = concentration;
                this.Volume = volume;
            }

            public Tuple<double, double> Dissociate()
            {
                List<double> results = new List<double>();
                for (int i = 0; i < this.DissociationConstants.Count; i++)
                {
                    double n = i == 0 ? this.Concentration : results.Last();
                    if (double.IsPositiveInfinity(this.DissociationConstants[i].Leading))
                    {
                        results.Add(n);
                        continue;
                    }

                    Tuple<double, double, double, double> ieqr = new Tuple<double, double, double, double>(
                        0.5 * (-this.DissociationConstants[i].K - Math.Sqrt(this.DissociationConstants[i].K) * Math.Sqrt(this.DissociationConstants[i].K + 4 * n)),
                        0.5 * (-this.DissociationConstants[i].K + Math.Sqrt(this.DissociationConstants[i].K) * Math.Sqrt(this.DissociationConstants[i].K + 4 * n)),
                        0.5 * (this.DissociationConstants[i].K + 2 * n - Math.Sqrt(this.DissociationConstants[i].K) * Math.Sqrt(this.DissociationConstants[i].K + 4 * n)),
                        0.5 * (this.DissociationConstants[i].K + 2 * n + Math.Sqrt(this.DissociationConstants[i].K) * Math.Sqrt(this.DissociationConstants[i].K + 4 * n)));

                    if (ieqr.Item1 < 0 || ieqr.Item1 > n || ieqr.Item4 < 0 || ieqr.Item4 > n)
                    {
                        results.Add(ieqr.Item2);
                    }
                    else
                    {
                        results.Add(ieqr.Item1);
                    }
                }

                double sum = results.Sum();
                return new Tuple<double, double>(sum, this.Concentration - sum);
            } 
        }

        [Serializable]
        public class DissociationConstant
        {
            [JsonProperty("Leading")]
            public double Leading { get; } = double.NaN;
            [JsonProperty("Exponent")]
            public double Exponent { get; } = double.NaN;

            public double K => this.Leading*Math.Pow(10, this.Exponent);
            public double pK => -Math.Log10(this.K);

            public DissociationConstant(double leading, double exponent)
            {
                this.Leading = leading;
                this.Exponent = exponent;
            }
        }
    }
}