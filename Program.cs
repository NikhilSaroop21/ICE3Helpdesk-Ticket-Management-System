using System;
using System.Collections.Generic;

public enum ActionType
{
	Create,
	Process,
	Modify,
	Delete
}

public class Ticket
{
	public int TicketNumber { get; private set; }
	public string Description { get; set; }
	public bool IsProcessed { get; set; }

	public Ticket(int ticketNumber, string description)
	{
		TicketNumber = ticketNumber;
		Description = description;
		IsProcessed = false;
	}

	public void Process()
	{
		IsProcessed = true;
	}

	public void UndoProcess()
	{
		IsProcessed = false;
	}
}

public class UndoAction
{
	public ActionType Type { get; private set; }
	public Ticket AffectedTicket { get; private set; }

	public UndoAction(ActionType type, Ticket ticket)
	{
		Type = type;
		AffectedTicket = ticket;
	}

	public void Undo()
	{
		switch (Type)
		{
			case ActionType.Process:
				AffectedTicket.UndoProcess();
				break;
				// Add cases for other types of actions if needed
		}
	}
}

public class TicketManager
{
	private Stack<UndoAction> undoStack;
	private Queue<Ticket> ticketQueue;
	private List<Ticket> ticketList;
	private Dictionary<int, Ticket> ticketDictionary;

	public TicketManager()
	{
		undoStack = new Stack<UndoAction>();
		ticketQueue = new Queue<Ticket>();
		ticketList = new List<Ticket>();
		ticketDictionary = new Dictionary<int, Ticket>();
	}

	public void CreateTicket(string description)
	{
		int ticketNumber = GenerateTicketNumber();
		Ticket newTicket = new Ticket(ticketNumber, description);
		ticketQueue.Enqueue(newTicket);
		ticketList.Add(newTicket);
		ticketDictionary.Add(ticketNumber, newTicket);
		undoStack.Push(new UndoAction(ActionType.Create, newTicket));
	}

	public void ProcessTicket()
	{
		if (ticketQueue.Count > 0)
		{
			Ticket nextTicket = ticketQueue.Dequeue();
			nextTicket.Process();
			undoStack.Push(new UndoAction(ActionType.Process, nextTicket));
			Console.WriteLine($"Ticket {nextTicket.TicketNumber} processed.");
		}
		else
		{
			Console.WriteLine("No tickets to process.");
		}
	}

	public void UndoAction()
	{
		if (undoStack.Count > 0)
		{
			UndoAction lastAction = undoStack.Pop();
			lastAction.Undo();
			Console.WriteLine("Last action undone.");
		}
		else
		{
			Console.WriteLine("Nothing to undo.");
		}
	}

	public void ViewTickets()
	{
		foreach (Ticket ticket in ticketList)
		{
			Console.WriteLine($"Ticket Number: {ticket.TicketNumber}, Description: {ticket.Description}, Processed: {ticket.IsProcessed}");
		}
	}

	public void SearchTicket(int ticketNumber)
	{
		if (ticketDictionary.ContainsKey(ticketNumber))
		{
			Ticket ticket = ticketDictionary[ticketNumber];
			Console.WriteLine($"Ticket Number: {ticket.TicketNumber}, Description: {ticket.Description}, Processed: {ticket.IsProcessed}");
		}
		else
		{
			Console.WriteLine("Ticket not found.");
		}
	}

	public void ModifyTicket(int ticketNumber, string newDescription)
	{
		if (ticketDictionary.ContainsKey(ticketNumber))
		{
			Ticket ticket = ticketDictionary[ticketNumber];
			ticket.Description = newDescription;
			undoStack.Push(new UndoAction(ActionType.Modify, ticket));
			Console.WriteLine($"Ticket {ticketNumber} description modified.");
		}
		else
		{
			Console.WriteLine("Ticket not found.");
		}
	}

	private int GenerateTicketNumber()
	{
		Random random = new Random();
		return random.Next(1000, 10000);
	}
}

class Program
{
	static void Main(string[] args)
	{
		TicketManager ticketManager = new TicketManager();

		while (true)
		{
			Console.WriteLine("\nHelpdesk Ticket Management System");
			Console.WriteLine("1. Create Ticket");
			Console.WriteLine("2. Process Ticket");
			Console.WriteLine("3. Undo Action");
			Console.WriteLine("4. View Tickets");
			Console.WriteLine("5. Search Ticket");
			Console.WriteLine("6. Modify Ticket");
			Console.WriteLine("7. Exit");
			Console.Write("Enter your choice: ");

			int choice;
			if (int.TryParse(Console.ReadLine(), out choice))
			{
				switch (choice)
				{
					case 1:
						Console.Write("Enter ticket description: ");
						string description = Console.ReadLine();
						ticketManager.CreateTicket(description);
						break;
					case 2:
						ticketManager.ProcessTicket();
						break;
					case 3:
						ticketManager.UndoAction();
						break;
					case 4:
						ticketManager.ViewTickets();
						break;
					case 5:
						Console.Write("Enter ticket number: ");
						int ticketNumber;
						if (int.TryParse(Console.ReadLine(), out ticketNumber))
						{
							ticketManager.SearchTicket(ticketNumber);
						}
						else
						{
							Console.WriteLine("Invalid ticket number.");
						}
						break;
					case 6:
						Console.Write("Enter ticket number to modify: ");
						int modifyTicketNumber;
						if (int.TryParse(Console.ReadLine(), out modifyTicketNumber))
						{
							Console.Write("Enter new description: ");
							string newDescription = Console.ReadLine();
							ticketManager.ModifyTicket(modifyTicketNumber, newDescription);
						}
						else
						{
							Console.WriteLine("Invalid ticket number.");
						}
						break;
					case 7:
						Environment.Exit(0);
						break;
					default:
						Console.WriteLine("Invalid choice. Please try again.");
						break;
				}
			}
			else
			{
				Console.WriteLine("Invalid choice. Please enter a number.");
			}
		}
	}
}
