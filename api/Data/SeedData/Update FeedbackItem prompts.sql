-- SQLite
UPDATE FeedbackItems SET Prompt1="Very Satisfied", Prompt2="Satisfied", Prompt3="Not Satisfied", Prompt4="Very Poor" 
Where QuestionNo=1 OR QuestionNo=2 OR QuestionNo=5 OR QuestionNo=7 OR QuestionNo=8;

UPDATE FeedbackItems SET Prompt1="Very Fast", Prompt2="Fast", Prompt3="Slow", Prompt4="Very Slow" 
Where QuestionNo=3 OR QuestionNo=6;


UPDATE FeedbackItems SET Prompt1="Very Good", Prompt2="Good", Prompt3="Not Good", Prompt4="Bad" 
Where QuestionNo=4;

UPDATE FeedbackItems SET Prompt1="Yes", Prompt2="No" Where QuestionNo=9;