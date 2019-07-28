﻿using Campus;
using Common;
using Faculty;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Companion behavior for the FacultyWindow
    /// </summary>
    public class FacultyWindow : Window
    {
        public Text TitleText;

        public RectTransform ScrollViewContent;

        public RectTransform ScrollViewRowTemplate;

        public Button CancelButton;

        public Text ClassroomUsageText;

        public Text LabUsageText;

        public Text TeachingScoreText;

        public Text ResearchScoreText;

        public override List<Button> Buttons => new List<Button> { CancelButton };

        public override void Open(object data)
        {
            CancelButton.OnSelect = () => { SelectionManager.UpdateSelection(null); };

            ClearList();
            PopulateList();

            UpdateStats();

            var camera = Camera.main.GetComponent<OrthoPanningCamera>();
            camera.FreezeCamera();
        }

        public override void Close()
        {
            var camera = Camera.main.GetComponent<OrthoPanningCamera>();
            camera.UnfreezeCamera();
        }

        public void UpdateStats()
        {
            FacultyManager manager = Accessor.Faculty;
            ClassroomUsageText.text = $"{manager.UsedClassroomCount}/{manager.AvailableClassroomCount}";
            LabUsageText.text = $"{manager.UsedLabsCount}/{manager.AvailableLabsCount}";
            TeachingScoreText.text = manager.TeachingScore.ToString();
            ResearchScoreText.text = manager.ResearchScore.ToString();
        }

        private void PopulateList()
        {
            foreach (HiredFaculty faculty in Accessor.Faculty.HiredFaculty)
            {
                RectTransform listItem = Instantiate(ScrollViewRowTemplate, ScrollViewContent);
                FacultyRow facultyRow = listItem.GetComponent<FacultyRow>();

                Sprite facultyHeadshot = Accessor.Faculty.GetHeadshotForFaculty(faculty.Id);
                facultyRow.Initialize(this, faculty, facultyHeadshot, Accessor);
                
                listItem.gameObject.SetActive(true);
            }
        }

        private void ClearList()
        {
            foreach (RectTransform listItem in ScrollViewContent)
            {
                if (listItem != ScrollViewRowTemplate)
                {
                    Destroy(listItem.gameObject);
                }
            }
        }
    }
}
