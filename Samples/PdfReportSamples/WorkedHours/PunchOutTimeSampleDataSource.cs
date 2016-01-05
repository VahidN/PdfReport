using System;
using System.Collections.Generic;
using PdfReportSamples.Models;

namespace PdfReportSamples.WorkedHours
{
    public class PunchOutTimeSampleDataSource
    {
        public static IList<PunchOutTimeRecord> GetLogTimesList()
        {
            return new List<PunchOutTimeRecord>
			{
				/* ------------------- Emp 1 ------------------- */
				new PunchOutTimeRecord /* ------ In ------ */
				{
					 Id = 1,
					 EmployeeName = "Emp 1",
					 LogTime = new DateTime(2011, 12, 5, 8, 9, 0)
				},
				new PunchOutTimeRecord /* ------ Out ------ */
				{
					 Id=1,
					 EmployeeName = "Emp 1",
					 LogTime = new DateTime(2011,12,5,10,10,0)
				},
				new PunchOutTimeRecord /* ------ In ------ */
				{
					 Id=1,
					 EmployeeName = "Emp 1",
					 LogTime = new DateTime(2011,12,5,11,2,0)
				},
				new PunchOutTimeRecord /* ------ Out ------ */
				{
					 Id=1,
					 EmployeeName = "Emp 1",
					 LogTime = new DateTime(2011,12,5,14,20,0)
				},
				new PunchOutTimeRecord /* ------ In ------ */
				{
					 Id=1,
					 EmployeeName = "Emp 1",
					 LogTime = new DateTime(2011,12,5,16,30,0)
				},
				new PunchOutTimeRecord /* ------ Out ------ */
				{
					 Id=1,
					 EmployeeName = "Emp 1",
					 LogTime = new DateTime(2011,12,5,18,9,0)
				},

				/* ------------------- Emp 2 ------------------- */
				new PunchOutTimeRecord /* ------ In ------ */
				{
					 Id=2,
					 EmployeeName = "Emp 2",
					 LogTime = new DateTime(2011,12,5,7,30,0)
				},
				new PunchOutTimeRecord /* ------ Out ------ */
				{
					 Id=2,
					 EmployeeName = "Emp 2",
					 LogTime = new DateTime(2011,12,5,9,10,0)
				},
				new PunchOutTimeRecord /* ------ In ------ */
				{
					 Id=2,
					 EmployeeName = "Emp 2",
					 LogTime = new DateTime(2011,12,5,10,52,0)
				},
				new PunchOutTimeRecord /* ------ Out ------ */
				{
					 Id=2,
					 EmployeeName = "Emp 2",
					 LogTime = new DateTime(2011,12,5,16,20,0)
				},

				/* ------------------- Emp 1 ------------------- */
				new PunchOutTimeRecord /* ------ In ------ */
				{
					 Id=1,
					 EmployeeName = "Emp 1",
					 LogTime = new DateTime(2011,12,6,8,10,0)
				},
				new PunchOutTimeRecord /* ------ Out ------ */
				{
					 Id=1,
					 EmployeeName = "Emp 1",
					 LogTime = new DateTime(2011,12,6,17,10,0)
				},

				/* ------------------- Emp 2 ------------------- */
				new PunchOutTimeRecord /* ------ In ------ */
				{
					 Id=2,
					 EmployeeName = "Emp 2",
					 LogTime = new DateTime(2011,12,6,7,35,0)
				},
				new PunchOutTimeRecord /* ------ Out ------ */
				{
					 Id=2,
					 EmployeeName = "Emp 2",
					 LogTime = new DateTime(2011,12,6,7,55,0)
				},
				new PunchOutTimeRecord /* ------ In ------ */
				{
					 Id=2,
					 EmployeeName = "Emp 2",
					 LogTime = new DateTime(2011,12,6,8,55,0)
				},				
				new PunchOutTimeRecord /* ------ Out ------ */
				{
					 Id=2,
					 EmployeeName = "Emp 2",
					 LogTime = new DateTime(2011,12,6,18,20,0)
				}
			};
        }
    }
}
