using BYSResults;
using HogWildSystem.DAL;
using HogWildSystem.ViewModels;
using System;
using System.Linq;

namespace HogWildSystem.BLL
{
    public class PartService
    {

        #region Fields

        /// <summary>
        /// The hog wild context
        /// </summary>
        private readonly HogWildContext _hogWildContext;

        #endregion

        // Constructor for the PartService class.
        internal PartService(HogWildContext hogWildContext)
        {
            // Initialize the _hogWildContext field with the provided HogWildContext instance.
            _hogWildContext = hogWildContext;
        }

        //	Get the parts
        public Result<List<PartView>> GetParts(int partCategoryID, string description, List<int> existingPartIDs)
        {
            // Create a Result container that will hold either a
            //	PartView objects on success or any accumulated errors on failure
            var result = new Result<List<PartView>>();
            #region Business Rules
            //	These are processing rules that need to be satisfied
            //		for valid data
            //		rule:	both part id must be valid and/or description cannot be empty
            //		rule: 	part IDs in existingPartIDs will be ignored 
            //		rule: 	RemoveFromViewFlag must be false

            if (partCategoryID == 0 && string.IsNullOrWhiteSpace(description))
            {
                result.AddError(new Error("Missing Information",
                    "Please provide either a category and/or description"));
                //  need to exit because we have no part information
                return result;
            }
            #endregion

            //  need to update description parameters so we are not searching on 
            //	 an empty value. Otherwise, this would return all records
            Guid tempGuild = Guid.NewGuid();
            if (string.IsNullOrWhiteSpace(description))
            {
                description = tempGuild.ToString();
            }

            //	ignore any parts that are in the "existing part ID" list
            //  ensure that we are compairing uppercase values for description
            var parts = _hogWildContext.Parts
                            .Where(p => !existingPartIDs.Contains(p.PartID)
                                && (description.Length > 0
                                && description != tempGuild.ToString()
                                && partCategoryID > 0
                                    ? (p.Description.ToUpper().Contains(description.ToUpper())
                                        && p.PartCategoryID == partCategoryID)
                                    : (p.Description.ToUpper().Contains(description.ToUpper())
                                        || p.PartCategoryID == partCategoryID)
                                && !p.RemoveFromViewFlag))
                        .Select(p => new PartView
                        {
                            PartID = p.PartID,
                            PartCategoryID = p.PartCategoryID,
                            CategoryName = p.PartCategory.Name,
                            Description = p.Description,
                            Cost = p.Cost,
                            Price = p.Price,
                            ROL = p.ROL,
                            QOH = p.QOH,
                            Taxable = (bool)p.Taxable,
                            RemoveFromViewFlag = p.RemoveFromViewFlag
                        })
                        .OrderBy(p => p.Description)
                        .ToList();

            //  if no parts were found
            if (parts == null || parts.Count == 0)
            {
                result.AddError(new Error("No parts", "No parts were found"));
                //  need to exit because we did not find any parts
                return result;
            }

            //  return the result
            return result.WithValue(parts);
        }
        //	Get the part
        public Result<PartView> GetPart(int partID)
        {
            // Create a Result container that will hold either a
            //	PartView objects on success or any accumulated errors on failure
            var result = new Result<PartView>();
            #region Business Rules
            //	These are processing rules that need to be satisfied
            //		rule:	partID must be valid
            //		rule: 	RemoveFromViewFlag must be false
            if (partID == 0)
            {
                result.AddError(new Error("Missing Information",
                                "Please provide a valid part id"));
                //  need to exit because we have no part information
                return result;
            }
            #endregion

            var part = _hogWildContext.Parts
                            .Where(p => (p.PartID == partID
                                         && !p.RemoveFromViewFlag))
                            .Select(p => new PartView
                            {
                                PartID = p.PartID,
                                PartCategoryID = p.PartCategoryID,
                                //  PartCategory is an alias for Lookup
                                CategoryName = p.PartCategory.Name,
                                Description = p.Description,
                                Cost = p.Cost,
                                Price = p.Price,
                                ROL = p.ROL,
                                QOH = p.QOH,
                                Taxable = (bool)p.Taxable,
                                RemoveFromViewFlag = p.RemoveFromViewFlag
                            }).FirstOrDefault();

            //  if no part were found
            if (part == null)
            {
                result.AddError(new Error("No part", "No part were found"));
                //  need to exit because we did not find any part
                return result;
            }

            //  return the result
            return result.WithValue(part);
        }
    }
}
