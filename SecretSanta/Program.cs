using System;
using System.IO;
using System.Collections;
using System.Net;
using System.Net.Mail;

/* people class contain people's name and email
* id is not really needed it was created to easily see if list is randomized
* 3 accessors to get each private field */
class People
{
    string name;
    string email;

    public People(string name, string email)
    {
        this.name = name;
        this.email = email;
    }

    public string Name
    {
        get { return name; }
    }

    public string Email
    {
        get { return email; }
    }
}

class Program
{
    // allow user input of string masked by asteriks
    static string GetPassword()
    {
        string password = "";
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(true);

            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
            {
                password += key.KeyChar;
                Console.Write("*");
            }
            else
            {
                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Substring(0, (password.Length - 1));
                    Console.Write("\b \b");
                }
            }
        }
        while (key.Key != ConsoleKey.Enter);

        return password;
    }

    static void Main()
    {
        // Server parameters
        //Server Name 	SMTP Address 	    Port 	SSL
        //Yahoo! 	smtp.mail.yahoo.com 	587 	Yes
        //GMail 	smtp.gmail.com 	587 	Yes
        //Hotmail 	smtp.live.com 	587 	Yes
        string smtpAddress = "smtp.mail.yahoo.com";
        int portNumber = 587;
        bool enableSSL = true;
        string emailFrom = "johnnyshih99@yahoo.com.tw";

        Console.Write("sending from {0}, enter password: ", emailFrom);
        string password = GetPassword();
        Console.WriteLine("\npassword entered");

        // get data from csv
        string filename = "people.csv";
        StreamReader sr = new StreamReader(filename);
        string line;
        string[] data;
        ArrayList ppl = new ArrayList();

        sr.ReadLine();
        while ((line = sr.ReadLine()) != null)
        {
            data = line.Split(',');
            ppl.Add(new People(data[1].Trim(), data[2].Trim()));
        }
        sr.Close();

        // randomize arraylist
        Random r = new Random();
        for (int i = 0; i < ppl.Count; i++)
        {
            object tmp = ppl[i];
            int idx = r.Next(ppl.Count - i) + i;
            ppl[i] = ppl[idx];
            ppl[idx] = tmp;
        }

        // after list randomized, send to current person (tmp) an email
        // informing about who he's secret santa to (tmpTo)
        for (int i = 0; i < ppl.Count; i++)
        {
            People tmp = null;
            People tmpTo = null;

            // creates a circle of secret santas
            if (i == ppl.Count - 1)
            {
                tmp = (People)ppl[i];
                tmpTo = (People)ppl[0];
            }
            else
            {
                tmp = (People)ppl[i];
                tmpTo = (People)ppl[i + 1];
            }

            // email content
            string emailTo = tmp.Email;
            string subject = "Secret Santa";
            string body = string.Format("Harro,\n{0}, you're secret santa to {1}! Please get him/her something sexy under $30!\n" +
                "***Please do NOT tell anyone who you got 'cuz that'll just ruin the surprise for everyone!***\n" +
                "Our secret santa party's on Dec. 2 at 6 pm, " +
                "so show up on time to 255 Keats Way, Unit 805 (buzzer code 2021). " +
                "If you don't know how secret santa works, feel free to ask June or Johnny :D" +
                "\nThanks!" +
                "\n\n*This is an automated message sent from an email account that Johnny never checks so please don't reply to this email.*" +
                "\n*Direct any questions via Facebook.*",
                tmp.Name, tmpTo.Name);

            // fail safe (keeping a record) 
            // in case email is not sent successfully
            StreamWriter w = new StreamWriter(tmp.Name + ".txt");
            w.WriteLine(body);
            w.Close();

            // send emails
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(emailFrom);
                mail.To.Add(emailTo);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = false;
                // Can set to false, if you are sending pure text.

                //mail.Attachments.Add(new Attachment("C:\\SomeFile.txt"));
                //mail.Attachments.Add(new Attachment("C:\\SomeZip.zip"));

                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    smtp.Credentials = new NetworkCredential(emailFrom, password);
                    smtp.EnableSsl = enableSSL;
                    Console.WriteLine("sending " + i + "...");
                    smtp.Send(mail);
                }
            }
        }

        //console output test
        /*for (int i = 0; i < ppl.Count; i++)
        {
            People tmp;
            People tmpTo;
            // creates a circle of secret santas
            if (i == ppl.Count - 1)
            {
                tmp = (People)ppl[i];
                tmpTo = (People)ppl[0];
            }
            else
            {
                tmp = (People)ppl[i];
                tmpTo = (People)ppl[i + 1];
            }
            Console.WriteLine("sending to {0}, he's secret santa to {1}", tmp.Name, tmpTo.Name);
        }*/
        Console.WriteLine("Completed");
        Console.ReadKey();
    }
}