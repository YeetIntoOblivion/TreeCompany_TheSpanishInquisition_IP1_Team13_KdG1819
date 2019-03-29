using System;
using System.Collections.Generic;
using DAL;
using Domain.UserInput;

namespace BL
{
    public class VoteManager
    {
        // Added by NVZ
        private IdeationVoteRepository voteRepo { get; set; }

        // Added by NVZ
        public VoteManager()
        {
            voteRepo = new IdeationVoteRepository();
        }
        
        // Added By NVZ
        // Edited by NG
        // Methods
        #region
        /*
         * This is needed for the voting feature. - NVZ
         */
        public bool handleVotingOnFeedback(int feedbackID, int userID, int? deviceID, double? x, double? y)
        {
            if (deviceID.HasValue)
            {
                int dID = (int) deviceID;
                
                if (!locationCheck(dID, x, y)) { return false; }
                voteRepo.Create(new Interaction() {UserId = userID, DeviceId = dID});
            }
            Vote obj = new Vote();
            obj.Id = feedbackID;
            
            List<float> coordinates = new List<float>();
            coordinates.Add((float) x);
            coordinates.Add((float) y);
            obj.SetLocation(coordinates);

            voteRepo.Create(obj);
            return true;
        }

        public bool locationCheck(int deviceId, double? x, double? y)
        {
            IOT_Device device = voteRepo.ReadDevice(deviceId);
            // deltas squared to make positive
            double? deltaX = device.LocationX - x;
            deltaX *= deltaX;
            double? deltaY = device.LocationY - y;
            deltaY *= deltaY;

            // arbitrary numbers
            if (deltaX < 10 && deltaY < 10) { return true; }
            return false;
        }
        #endregion
    }
}