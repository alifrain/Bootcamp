using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ef.Data;
using ef.Models;

using var db = new MyDbContext();

var firstEmp = db.Employees.First();
var proj = new Project { Title = "CRM Revamp" };
proj.Employees.Add(firstEmp);
db.Projects.Add(proj);
db.SaveChanges();

var projects = db.Projects.Include(p => p.Employees).ToList();
foreach (var p in projects)
    Console.WriteLine($"{p.Title} => {string.Join(", ", p.Employees.Select(e => e.Name))}");

