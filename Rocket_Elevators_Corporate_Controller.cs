using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Rocket_Elevators_Corporate_Controller
//current building scenario 85 floors  4 columns 5  elevator cages for each column.


//Method 1: RequestElevator(FloorNumber, RequestedFloor)
//Method 2: AssignElevator(RequestedFloor)




namespace ConsoleApp1
{

    // -----------------------------------------------------------------------------class controller 
    public class Controller
    {
        public int nb_floor;
        public int nb_elevators;
        public int nb_column;
        public List<Column> column_list;
        public int nb_elevator_column;
        public Elevator GoodElevator;
        public Elevator nearestElevator;

        public Controller(int nb_floor, int nb_elevators, int nb_column)
        {

            this.nb_floor = nb_floor;
            this.nb_elevators = nb_elevators;
            this.nb_column = nb_column;
            this.column_list = new List<Column>();
            this.nb_elevator_column = nb_elevators / nb_column; //modulo for float numbers

            for (int i = 0; i < this.nb_column; i++)
            {
                int gap = Calculate_column();
                int range_start = (i * gap) + 1;
                int range_stop = (i * gap) + gap;

                if (i == this.nb_column - 1)
                {
                    // add difference (modulo) to the last section of floor for the last column
                    range_stop += Remaining_floor();
                }
                column_list.Add(new Column(this.nb_elevators, nb_elevator_column, range_start, range_stop));

            }

        }

        // calculate range floor for each column
        public int Calculate_column()
        {
            float float_gap = nb_floor / nb_column;
            int new_gap = (int)Math.Floor(float_gap);
            return new_gap;
        }


        // calculate difference (modulo) to the last section of floor for the last column
        public int Remaining_floor()
        {
            int modulo_extra_floor = nb_floor % nb_column;
            return modulo_extra_floor;
        }

    
        // determine the good column 
        public Column find_column(int RequestFloor)
        {            
            Column goodColumn = null;
            foreach (Column column in column_list)
            {
                if (column.range_start <= RequestFloor && column.range_stop >= RequestFloor)
                {
                    goodColumn = column;
                }
            }
            return goodColumn;
        }
        

        // nearest elevator calcul !!!!!
        public Elevator Nearest_elevator(int nb_floor, int current_floor, int RequestFloor)
        {
            int reference_gap = 1000;
            this.nearestElevator = null;
            Column column = find_column(RequestFloor);

            foreach (Elevator elevator in column.elevator_list)
            {
                int calculate_gap = Math.Abs(nb_floor - current_floor);

                if (calculate_gap <= reference_gap)
                {
                    reference_gap = calculate_gap;
                    nearestElevator = GoodElevator;
                }
            }
            return nearestElevator;
        }

        //find best elevator 
        public Elevator AssignElevator(int nb_floor, int RequestFloor)
        {
            Column column = find_column(RequestFloor);
            this.GoodElevator = column.elevator_list[0];

            foreach (Elevator elevator in column.elevator_list)
            {
                if (nb_floor == elevator.current_floor && elevator.Status == "Stopped")
                {
                    GoodElevator = elevator;

                }

                else if (nb_floor == elevator.current_floor && elevator.Status == "IDLE")
                {
                    GoodElevator = elevator;
                }

                else if (nb_floor > elevator.current_floor && (elevator.Status == "moving" || elevator.Status == "Stopped") && elevator.Direction == "Up")
                {
                    GoodElevator = Nearest_elevator(nb_floor, elevator.current_floor, RequestFloor);
                }

                else if (nb_floor < elevator.current_floor && (elevator.Status == "moving" || elevator.Status == "Stopped") && elevator.Direction == "Down")
                {
                    GoodElevator = Nearest_elevator(nb_floor, elevator.current_floor, RequestFloor);
                }

                else if (nb_floor != elevator.current_floor && elevator.Status == "IDLE")
                {
                    GoodElevator = elevator;
                }

            }
            
            return GoodElevator;
        }



        // Request button !!!inside!!! elevator 
        public Elevator RequestFloor(Elevator elevator, int nb_floor)
        {
            elevator.floor_list.Add(nb_floor);
            elevator.operate(nb_floor);
            return elevator;
        }


        // request panel !!!outside!!! elevator
        public void RequestElevator(int nb_floor, int RequestFloor)
        {
            Elevator best_elevator = AssignElevator(nb_floor, RequestFloor);
            Console.Write(best_elevator);
            this.RequestFloor(best_elevator, nb_floor);
        }


    }


    // -----------------------------------------------------------------------------class column in controller
    public class Column
    {
        public List<Elevator> elevator_list;
        public int range_start;
        public int range_stop;

        public Column(int column, int nb_elevator_column, int rang_start, int range_stop)
        {
            this.elevator_list = new List<Elevator>();
            this.range_start = range_start;
            this.range_stop = range_stop;
            

            for (int i = 0; i < nb_elevator_column; i++)
            {
                this.elevator_list.Add(new Elevator("Elevator " + i));
            }

        }

    }


    // -----------------------------------------------------------------------------class elevator in column in controller
    public class Elevator
    {
        public int best_elevator;
        public string Status;
        public int current_floor;
        public string Direction;
        public List<int> floor_list;
        public int elevator;

        public Elevator(string elevatorName)
        {
            this.best_elevator = best_elevator;
            this.Status = "stopped";
            this.current_floor = 1;
            this.Direction = "";
            this.floor_list = new List<int>();

        }

        public void Open_door()
        {
            Console.Write("OpenDoor");
        }
        public void Close_door()
        {
            Console.Write("CloseDoor");
        }


        //# display floor info 1,2,3,4... and make the elevator moove
        public void move_show()
        {
            string Status = "";
            if (this.floor_list[0] > this.current_floor)
            {
                while (this.floor_list[0] != this.current_floor)
                {
                    
                    current_floor += 1;
                    Console.Write("current floor", current_floor);
                    Status = "Moving up";
                    Console.Write("Elevator State:", Status);
                    System.Threading.Thread.Sleep(800);
                    Console.Write("Elevator arrived to floor:", current_floor);
                }
            }

            else if (this.floor_list[0] < this.current_floor)
            {
                while (this.floor_list[0] != this.current_floor)
                {
                    current_floor -= 1;
                    Console.Write("current floor", current_floor);
                    Status = "Moving down";
                    Console.Write("Elevator State:", Status);
                    System.Threading.Thread.Sleep(800);
                    Console.Write("Elevator arrived to floor:", current_floor);
                }
            }
            else
            {
                Console.Write("Elevator has arrived to floor:", current_floor);
                Status = "Stop";
            }

        }


        // operate the elavater doors and movement with move_show
        public void operate(int nb_floor)
        {
            //Console.Write("operate_elevator", GoodElevator, nb_floor);
            //Console.Write(GoodElevator)
            this.move_show();
            this.Open_door();
            System.Threading.Thread.Sleep(800);
            this.Close_door();
            System.Threading.Thread.Sleep(800);
        }


    }


    //  -----------------------------------------------------------------------------class executer
    class Program
    {
        static void Main(string[] args)
        {
            Controller controller_1 = new Controller(85, 20, 4);
            controller_1.column_list[1].elevator_list[0].current_floor = 1;
            controller_1.AssignElevator(1, 24);
            controller_1.AssignElevator(1, 28);
            Console.WriteLine("hello");

            Console.WriteLine(controller_1.column_list[0]);
            Console.WriteLine(controller_1.column_list[0].elevator_list[0]);



        }
    }

}

