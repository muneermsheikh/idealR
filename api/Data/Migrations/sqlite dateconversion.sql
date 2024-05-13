-- SQLite
SELECT Id, BCCEmail, CCEmail, Content, DateRead, MessageSent, MessageType, RecipientAppUserId, RecipientDeleted, RecipientEmail, RecipientId, RecipientUsername, SenderAppUserId, SenderDeleted, SenderEmail, SenderId, SenderUsername, Subject
FROM Messages;

UPDATE Messages SET MessageSentOn = strftime('%Y-%m-%d', MessageSentOn)