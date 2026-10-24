BEGIN TRANSACTION;

UPDATE Students         SET Section = REPLACE(Section, 'Class ', '') WHERE Section LIKE 'Class %';
UPDATE ClassAssignments SET Section = REPLACE(Section, 'Class ', '') WHERE Section LIKE 'Class %';
UPDATE Teachers         SET Section = REPLACE(Section, 'Class ', '') WHERE Section LIKE 'Class %';

COMMIT TRANSACTION;

SELECT DISTINCT GradeLevel, Section FROM Students ORDER BY GradeLevel, Section;