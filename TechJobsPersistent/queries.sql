--Part 1
DESCRIBE jobs;
--Part 2
SELECT Name FROM employers WHERE Location = "Saint Louis";
--Part 3
SELECT DISTINCT Name, Description FROM skills RIGHT JOIN jobskills ON skills.Id = jobskills.SkillId ORDER BY Name;
