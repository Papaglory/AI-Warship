using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipGame.Ship.Statistics
{
    public class GraphTemplate {

        const float EULER = 2.718f;

        bool positiveSlopeSign = false;
        bool aggressiveSlope = false;
        bool eulerSlope = false;
        float eulerStrength;
        bool reachingLimitSlope = false;
        int limitSlopeStrength;


        public GraphTemplate(bool _positiveSlopeSign, bool _aggressiveSlope, bool _eulerSlope, float _eulerStrength, bool _reachingLimitSlope, int _limitSlopeStrength)
        {
            positiveSlopeSign = _positiveSlopeSign;
            aggressiveSlope = _aggressiveSlope;
            eulerSlope = _eulerSlope;
            eulerStrength = _eulerStrength;
            reachingLimitSlope = _reachingLimitSlope;
            limitSlopeStrength = _limitSlopeStrength;
        }

        public int SlopeCalculation(int _score)
        {
            float newScore = 0;
            bool _positiveSlopeSign = FindSlopeSign();

            if (reachingLimitSlope)
            {
                //bruk denne type graph
                if (_positiveSlopeSign)
                {
                    //Bruker x-2
                    newScore = (100 * _score * _score) / (_score * _score + limitSlopeStrength);
                    return Mathf.RoundToInt(newScore);

                }
                else
                {
                    //Bruker -x-2
                    newScore = 100 - ((100 * _score * _score) / (_score * _score + limitSlopeStrength));
                    return Mathf.RoundToInt(newScore);
                }
            }

            //TODO ER EIN FEIL MED DEN SISTE ELSE-EN MÅ KANSKJE HA RETURN NEWSCORE PÅ ALLE
            if (eulerSlope)
            {
                if (_positiveSlopeSign)
                {
                    //Bruker positiv euler
                    //FUNKER APPROVED
                    float aboveDivision = EULER;
                    float underDivision = _score / eulerStrength;
                    newScore = Mathf.Pow(aboveDivision, underDivision);
                    Mathf.RoundToInt(newScore);
                    return Mathf.RoundToInt(newScore);
                }
                else
                {
                    //bruker negativ euler
                    newScore = 100 - Mathf.Pow(EULER, (_score / eulerStrength));
                    return Mathf.RoundToInt(newScore);
                }
            }

            if (aggressiveSlope)
            {
                if (_positiveSlopeSign)
                {
                    //Bruker x^2 slope
                    //newScore = (score * score) * (1 / 100);
                    newScore = Mathf.Sqrt(100 * _score) - 10;
                    return Mathf.RoundToInt(newScore);
                }
                else
                {
                    //Bruker -x^2 Slope
                    newScore = 100 - Mathf.Sqrt(100 * _score);
                    return Mathf.RoundToInt(newScore);
                }
            }
            else
            {
                if (_positiveSlopeSign)
                {
                    //Bruker sqrt(x)
                    newScore = 5 + ((_score * _score) / 120);
                    return Mathf.RoundToInt(newScore);
                    
                }
                else
                {
                    //Bruker -sqrt(x)
                    newScore = 100 - ((_score * _score) / (100));
                    return Mathf.RoundToInt(newScore);                    
                }
            }
        }

        private bool FindSlopeSign()
        {
            bool _positiveSlopeSign;
            if (positiveSlopeSign)
            {
                _positiveSlopeSign = true;
            }
            else
            {
                _positiveSlopeSign = false;
            }

            return _positiveSlopeSign;
        }
    }
}