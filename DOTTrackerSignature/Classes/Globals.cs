using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace IdslTracker
{
    public static class Globals
    {
        internal static List<string> ManfSites;

        internal static List<string> PrintedByNames;

        internal static List<string> ManufactureReps;

        public static bool IsPowerUser { get; internal set; }
        public static bool IsAdGrpTrackerUsers { get; internal set; }
        public static MainWindow TrackerWindow { get; internal set; }
      
        public static Color? lastDoorCommentColour { get; internal set; }
        public static Color? lastFrameCommentColour { get; internal set; }
        public static Uri LoadingAnimPath { get; internal set; }

        public static List<string> HeaderDates;
    }
}