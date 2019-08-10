﻿using Common;
using GameData;
using System.Collections.Generic;
using UnityEngine;

namespace Faculty
{
    public class FacultyGenerator
    {
        private readonly Sprite _defaultHeadshot;
        private readonly List<string> _firstNamesMen;
        private readonly List<string> _firstNamesWomen;
        private readonly List<string> _lastNames;

        private int _nextFacultyId = 0;

        public FacultyGenerator(FacultyData data)
        {
            _defaultHeadshot = data.DefaultHeadshot;
            _firstNamesMen = data.FirstNamesMen.Names;
            _firstNamesWomen = data.FirstNamesWomen.Names;
            _lastNames = data.LastNames.Names;
        }

        public void SetNextFacultyId(int id)
        {
            if (id < _nextFacultyId)
            {
                GameLogger.Error("Unexpected next faculty id set! Current = {0}; New = {1};", _nextFacultyId, id);
            }

            _nextFacultyId = id;
        }

        public GeneratedFaculty GenerateNext()
        {
            bool isWoman = Random.Range(0, 2) % 2 == 0;
            List<string> firstNames = isWoman ? _firstNamesWomen : _firstNamesMen;
            int firstIndex = Random.Range(0, firstNames.Count);
            int lastIndex = Random.Range(0, _lastNames.Count);
            string name = $"{firstNames[firstIndex]} {_lastNames[lastIndex]}";

            int teachingScore = Random.Range(50, 100);
            int researchScore = Random.Range(50, 100);

            int maximumSlots = 5; /* TODO: What to do with this? */

            int salary = ((teachingScore + researchScore) * maximumSlots) * 100;
            salary += Random.Range(-20000, 20001);
            salary = (salary / 5000) * 5000;

            var newFaculty = new GeneratedFaculty(
                _nextFacultyId++,
                name,
                salary,
                teachingScore,
                researchScore,
                maximumSlots);

            GameLogger.Info("Generated new faculty. {0}", newFaculty);
            return newFaculty;
        }

        public Sprite GetHeadshotForFaculty(int id)
        {
            // TODO: I want to generate random headshots for each faculty. Mr. Potato Head style.
            return _defaultHeadshot;
        }
    }
}