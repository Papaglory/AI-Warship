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

        public float SlopeCalculation(float _score)
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
                    
                }
                else
                {
                    //Bruker -x-2
                    newScore = 100 - ((100 * _score * _score) / (_score * _score + limitSlopeStrength));
                }
            }

            //TODO ER EIN FEIL MED DEN SISTE ELSE-EN MÅ KANSKJE HA RETURN NEWSCORE PÅ ALLE
            if (eulerSlope)
            {
                if (_positiveSlopeSign)
                {
                    //Bruker positiv euler
                    float aboveDivision = EULER;
                    float underDivision = _score / eulerStrength;
                    newScore = Mathf.Pow(aboveDivision, underDivision);
                    Debug.Log("DONE " + newScore);
                    return newScore;
                }
                else
                {
                    //bruker negativ euler
                    newScore = 100 - Mathf.Pow(EULER, (_score / eulerStrength));
                }
            }

            if (aggressiveSlope)
            {
                if (_positiveSlopeSign)
                {
                    //Bruker x^2 slope
                    //newScore = (score * score) * (1 / 100);
                    newScore = (_score * _score)/100;
                }
                else
                {
                    //Bruker -x^2 Slope
                    newScore = 100 + (_score * _score) * (1 / 100);
                }
            }
            else
            {
                if (_positiveSlopeSign)
                {
                    //Bruker sqrt(x)
                    newScore = Mathf.Sqrt(100 * _score);
                }
                else
                {
                    //Bruker -sqrt(x)
                    newScore = -Mathf.Sqrt(100 * _score) + 100;
                }
            }
            return newScore;
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