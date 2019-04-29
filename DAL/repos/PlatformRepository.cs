using System.Collections.Generic;
using System.Data;
using System.Linq;
using Domain.Users;
using DAL.Contexts;
using DAL.Data_Transfer_Objects;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class PlatformRepository : IRepository<Platform>
    {
        // Added by DM
        // Modified by NVZ
        private CityOfIdeasDbContext ctx;

        // Added by NVZ
        public PlatformRepository()
        {
            ctx = new CityOfIdeasDbContext();
        }

        // Added by NVZ
        // Standard Methods
        #region
        private PlatformsDTO ConvertToDTO(Platform p)
        {
            return new PlatformsDTO
            {
                PlatformID = p.Id,
                Name = p.Name,
                SiteUrl = p.Url
                // TODO: (SPRINT2?) Dit kunnen oplossen
                // IconImage = p.Image 
            };
        }

        private Platform ConvertToDomain(PlatformsDTO DTO)
        {
            return new Platform
            {
                Id = DTO.PlatformID,
                Name = DTO.Name,
                Url = DTO.SiteUrl
                // TODO: (SPRINT2?) Dit kunnen oplossen
                // IconImage = p.Image 
            };
        }
        
        // Added by XV
        // Select the biggest current Id from the platforms and increment it by on -XV
        private int FindNextAvailablePlatformId()
        {               
            int newId = ReadAll().Max(platform => platform.Id)+1;
            return newId;
        }
        #endregion
        
        // Added by NVZ
        // Platform CRUD
        #region
        public Platform Create(Platform obj)
        {
            IEnumerable<Platform> platforms = ReadAll();

            foreach (Platform p in platforms)
            {
                if (ExtensionMethods.HasMatchingWords(p.Name, obj.Name) > 0)
                {
                    throw new DuplicateNameException("Platform(ID=" + obj.Id + ") met naam " + obj.Name + " heeft dezelfde naam als Platform(ID=" + p.Id +
                        " met naam " + p.Name);
                }
            }

            obj.Id = FindNextAvailablePlatformId();
            ctx.Platforms.Add(ConvertToDTO(obj));
            ctx.SaveChanges();
           
            return obj;
        }

        public Platform Read(int id, bool details)
        {
            PlatformsDTO platformDTO = null;
            platformDTO = details ? ctx.Platforms.AsNoTracking().First(p => p.PlatformID == id) : ctx.Platforms.First(p => p.PlatformID == id);
            ExtensionMethods.CheckForNotFound(platformDTO, "Platform", id);

            return ConvertToDomain(platformDTO);
        }

        
        // Modified by XV & NVZ
        public void Update(Platform obj)
        {
            PlatformsDTO newPlatform = ConvertToDTO(obj);
            PlatformsDTO foundPlatform = ctx.Platforms.FirstOrDefault(dto => dto.PlatformID == newPlatform.PlatformID);
            if (foundPlatform != null)
            {
                foundPlatform.Name = newPlatform.Name;
                foundPlatform.SiteUrl = newPlatform.SiteUrl;
                foundPlatform.IconImage = newPlatform.IconImage;
                ctx.Platforms.Update(foundPlatform);
            }

            ctx.SaveChanges();
        }

        public void Delete(int id)
        {
            PlatformsDTO toDelete = ctx.Platforms.First(r => r.PlatformID == id);
            ctx.Platforms.Remove(toDelete);
            ctx.SaveChanges();
        }

        public IEnumerable<Platform> ReadAll()
        {
            List<Platform> myQuery = new List<Platform>();

            foreach (PlatformsDTO DTO in ctx.Platforms)
            {
                myQuery.Add(ConvertToDomain(DTO));
            }

            return myQuery;
        }
        #endregion  
    }
}