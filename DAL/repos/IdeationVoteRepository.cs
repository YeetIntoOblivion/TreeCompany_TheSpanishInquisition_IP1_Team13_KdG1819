using System.Collections.Generic;
using Domain.UserInput;
using DAL.Contexts;
using System.Data;
using DAL.Data_Transfer_Objects;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DAL
{
    public class IdeationVoteRepository : IRepository<Vote>
    {
        // Added by NVZ
        private CityOfIdeasDbContext ctx;

        public IdeationVoteRepository()
        {
            ctx = new CityOfIdeasDbContext();
        }

        // Added by NVZ
        // Standard Methods
        #region
        private VotesDTO ConvertToDTO(Vote obj)
        {
            return new VotesDTO
            {
                VoteID = obj.Id,
                DeviceID = obj.Device.Id,
                InputID = obj.Idea.Id,
                InputType = 2, //Voorlopig Idee
                UserMail = obj.UserMail,
                LocationX = obj.LocationX,
                LocationY = obj.LocationY,
                Choices = ExtensionMethods.ListToString(obj.Choices)
            };
        }    

        private Vote ConvertToDomain(VotesDTO DTO)
        {
            return new Vote
            {
                Id = DTO.VoteID,
                Device = new IOT_Device { Id = DTO.DeviceID },
                Idea = new Idea { Id = DTO.InputID },
                UserMail = DTO.UserMail,
                LocationX = DTO.LocationX,
                LocationY = DTO.LocationY,
                Choices = ExtensionMethods.StringToList(DTO.Choices)
            };
        }

        private DevicesDTO ConvertToDTO(IOT_Device obj)
        {
            return new DevicesDTO
            {
                DeviceID = obj.Id,
                LocationX = obj.LocationX,
                LocationY = obj.LocationY
            };
        }

        private IOT_Device ConvertToDomain(DevicesDTO DTO)
        {
            return new IOT_Device
            {
                Id = DTO.DeviceID,
                LocationX = DTO.LocationX,
                LocationY = DTO.LocationY
            };
        }
        #endregion

        // Added by NVZ
        // Vote CRUD
        #region
        public Vote Create(Vote obj)
        {
            IEnumerable<Vote> votes = ReadAll(obj.Device.Id);

            foreach (Vote v in votes)
            {
                if(v.UserMail == obj.UserMail)
                {
                    throw new DuplicateNameException("Vote(ID=" + obj.Id + ") en Vote(ID=" + v.Id + ") hebben dezelfde email en Device(ID=" + 
                        obj.Device.Id + "), dit is absoluut niet toegestaan!");
                }
            }

            ctx.Votes.Add(ConvertToDTO(obj));
            ctx.SaveChanges();

            return obj;
        }
        
        public Vote Read(int id, bool details)
        {
            VotesDTO voteDTO = null;

            if (details)
            {
                voteDTO = ctx.Votes.AsNoTracking().First(p => p.VoteID == id);
                ExtensionMethods.CheckForNotFound(voteDTO, "Vote", voteDTO.VoteID);
            }
            else
            {
                voteDTO = ctx.Votes.First(p => p.VoteID == id);
                ExtensionMethods.CheckForNotFound(voteDTO, "Vote", voteDTO.VoteID);
            }

            return ConvertToDomain(voteDTO);
        }
        
        public void Update(Vote obj)
        {
            VotesDTO newVote = ConvertToDTO(obj);
            Vote found = Read(obj.Id, false);
            VotesDTO foundVote = ConvertToDTO(found);
            foundVote = newVote;
            ctx.SaveChanges();
        }
        
        public void Delete(int id)
        {
            Vote toDelete = Read(id, false);
            ctx.Votes.Remove(ConvertToDTO(toDelete));
            ctx.SaveChanges();
        }
        
        public IEnumerable<Vote> ReadAll()
        {
            List<Vote> myQuery = new List<Vote>();

            foreach (VotesDTO DTO in ctx.Votes)
            {
                myQuery.Add(ConvertToDomain(DTO));
            }

            return myQuery;
        }

        public IEnumerable<Vote> ReadAll(int deviceID)
        {
            return ReadAll().ToList().FindAll(vote => vote.Device.Id == deviceID);
        } 
        #endregion
               
        
        // Added by NVZ
        // Devices CRUD
        #region
        public IOT_Device Create(IOT_Device obj)
        {
            IEnumerable<IOT_Device> devices = ReadAllDevices();

            foreach (IOT_Device iot in devices)
            {
                if (iot.LocationX == obj.LocationX && iot.LocationY == obj.LocationY)
                {
                    throw new DuplicateNameException("Device(ID=" + iot.Id + ") en Device(ID=" + obj.Id + ") delen dezelfde locatie nl. " + obj.LocationX +
                        "," + obj.LocationY + ".");
                }
            }

            ctx.Devices.Add(ConvertToDTO(obj));
            ctx.SaveChanges();

            return obj;
        }

        public IOT_Device ReadDevice(int deviceID, bool details)
        {
            DevicesDTO deviceDTO = null;

            if (details)
            {
                deviceDTO = ctx.Devices.AsNoTracking().First(d => d.DeviceID == deviceID);
                ExtensionMethods.CheckForNotFound(deviceDTO, "IOT_Device", deviceID);
            }
            else
            {
                deviceDTO = ctx.Devices.First(d => d.DeviceID == deviceID);
                ExtensionMethods.CheckForNotFound(deviceDTO, "IOT_Device", deviceID);
            }

            return ConvertToDomain(deviceDTO);
        }

        public void Update(IOT_Device obj)
        {
            DevicesDTO newDevice = ConvertToDTO(obj);
            IOT_Device found = ReadDevice(obj.Id, false);
            DevicesDTO foundDevice = ConvertToDTO(found);
            foundDevice = newDevice;
            ctx.SaveChanges();
        }

        public void DeleteDevice(int id)
        {
            IOT_Device toDelete = ReadDevice(id, false);
            ctx.Devices.Remove(ConvertToDTO(toDelete));
            ctx.SaveChanges();
        }

        public IEnumerable<IOT_Device> ReadAllDevices()
        {
            List<IOT_Device> myQuery = new List<IOT_Device>();

            foreach (DevicesDTO DTO in ctx.Devices)
            {
                myQuery.Add(ConvertToDomain(DTO));
            }

            return myQuery;
        }
        #endregion
    }
}