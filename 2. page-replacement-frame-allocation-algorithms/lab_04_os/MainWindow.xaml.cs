using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;
using System.Diagnostics;

namespace lab_04_os
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<Process> processes = new List<Process>();
        List<PieChart> pies = new List<PieChart>();
        List<TextBox> textBoxes = new List<TextBox>();
        Process process1 = new Process();
        Process process2 = new Process();
        Process process3 = new Process();
        Process process4 = new Process();
        int mmu = 0;
        int sum = 0;

        

        public MainWindow()
        {
            InitializeComponent();            
            addProcesses();          
            
        }

        private void addProcesses()
        {
            processes.Add(process1);
            processes.Add(process2);
            processes.Add(process3);
            processes.Add(process4);

            

            pies.Add(RandomPie); //0
            pies.Add(ProportionalPie); //1
            pies.Add(PageFaultPie); //2
            pies.Add(WorkingSetPie); //3



            textBoxes.Add(p1);
            textBoxes.Add(p2);
            textBoxes.Add(p3);
            textBoxes.Add(p4);
        }

        private void generateData()
        {
            RandomAlgo();
            ProportionalAlgo();
            WorkingSetAlgo();
            PageFaultAlgo();

        }

        private void showData(int idx, int[] arr)
        {
            pies[idx].Series.Add(new PieSeries { Title = $"Process 1  ", Fill = Brushes.Blue, StrokeThickness = 0, Values = new ChartValues<int> { arr[0] } });
            pies[idx].Series.Add(new PieSeries { Title = $"Process 2  ", Fill = Brushes.Green, StrokeThickness = 0, Values = new ChartValues<int> { arr[1] } });
            pies[idx].Series.Add(new PieSeries { Title = $"Process 3  ", Fill = Brushes.Red, StrokeThickness = 0, Values = new ChartValues<int> { arr[2] } });
            pies[idx].Series.Add(new PieSeries { Title = $"Process 4  ", Fill = Brushes.Orange, StrokeThickness = 0, Values = new ChartValues<int> { arr[3] } });
            pies[idx].Series.Add(new PieSeries { Title = $"Remaining  ", Fill = Brushes.Gray, StrokeThickness = 0, Values = new ChartValues<int> { arr[4] } });
        }
        

        private void PageFaultAlgo()
        {

            int[] arr = new int[5];
            decimal pages;
            decimal pagesAdded;
            int pageFaults = 0;
            int spaceAll = 0;
            int remaining = 0;

            for (int i = 0; i < processes.Count(); i++) //adding pages with priority only
            {
                pages = (decimal) processes[i].getPages();
                pagesAdded = (decimal) pToAllProp(processes[i], mmu) * (1 / 4);

                if (processes[i].getPriority() && spaceAll < mmu)
                {
                    arr[i] = min((int) (pages + pagesAdded), mmu-spaceAll);
                    spaceAll += arr[i];
                }
            }

            decimal pagesRemoved;

            for (int i = 0; i < processes.Count(); i++) //adding pages with priority only
            {
                pages = (decimal)processes[i].getPages();
                pagesRemoved = (decimal)pToAllProp(processes[i], mmu) * (1 / 4);

                if (!processes[i].getPriority() && spaceAll < mmu)
                {
                    arr[i] = min((int)(pages - pagesRemoved), mmu - spaceAll);
                    spaceAll += arr[i];
                }
            }

            for (int i = 0; i < processes.Count(); i++)
            {
                if (processes[i].getPages() - arr[i] > 0) pageFaults += processes[i].getPages() - arr[i];

            }


            arr[4] = mmu-spaceAll;
            showData(2, arr);
            if (pageFaults < 0) pageFaults = 0;
            pageFaultLabel.Content = $"Page faults: {pageFaults}";

        }

        private int min(int a, int b)
        {
            if (a < b) return a;
            if (b < a) return b;
            return a;
        }
        private void WorkingSetAlgo()
        {
            int[] arr = new int[5];
            int spaceAll = 0;
            int pageFaults = 0;
            int remaining = 0;
            for (int i=0; i<processes.Count(); i++) //adding pages with priority only
            {
                if (processes[i].getPriority() && spaceAll<mmu)
                {
                    arr[i] = min(processes[i].getPages(), mmu - spaceAll);
                    spaceAll += arr[i];

                }
            }

            int memoryAvailable = mmu - spaceAll;

            if (memoryAvailable>0)
            {
                
                for (int i = 0; i < processes.Count(); i++) 
                {
                    if (arr[i] == 0)
                    {

                        arr[i] = pToAllProp(processes[i], memoryAvailable);
                        spaceAll += arr[i];
                    }
                }
            }

            for (int i = 0; i < processes.Count(); i++)
            {
                if(processes[i].getPages() - arr[i]>0) pageFaults += processes[i].getPages() - arr[i];               
            }

            arr[4] = mmu - spaceAll;
            showData(3, arr);
            if (pageFaults < 0) pageFaults = 0;
            workingSetLabel.Content = $"WorkingSet: {pageFaults}";
        }

        private int pToAllProp(Process p, int memory)
        {
            decimal memoryAvailable = (decimal)memory;
            decimal pages = (decimal)p.getPages();
            decimal sumOfPages = (decimal)sum;
            decimal pagesToReturn =memoryAvailable * (pages / sumOfPages);
            return (int)pagesToReturn;
        }
        private void ProportionalAlgo()
        {
            int pageFaults = 0;
            int remaining = 0;
            int spaceAll = 0;

            int[] arr = { pToAllProp(process1, mmu), pToAllProp(process2, mmu), pToAllProp(process3, mmu), pToAllProp(process4, mmu), remaining};
            for (int i = 0; i < processes.Count(); i++)
            {
                if (processes[i].getPages() - arr[i] > 0) pageFaults += processes[i].getPages() - arr[i];
                spaceAll += arr[i];

            }
            arr[4] = mmu - spaceAll;
            showData(1, arr);
            if (pageFaults < 0) pageFaults = 0;
            propLabel.Content = $"Proportional: {pageFaults}";
        }

        private void RandomAlgo()
        {
            int[] arr = new int[5];
            Random rand = new Random();
            int pageFaults = 0;
            int remaining = sum;
            int spaceAll = 0;
            for(int i=0; i<arr.Length-1; i++)
            {
                if (spaceAll < mmu)
                {
                    arr[i] = rand.Next(0, processes[i].getPages());
                    pageFaults += processes[i].getPages() - arr[i];
                    spaceAll += arr[i];
                }
            }


            arr[4] = mmu - spaceAll>0 ? mmu - spaceAll :0;
            showData(0,arr);
            if (pageFaults < 0) pageFaults = 0;
            randomLabel.Content = $"Random: {pageFaults}";

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void GridMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Random_Click(object sender, RoutedEventArgs e)
        {
            Random rand = new Random();
            foreach(TextBox box in textBoxes) box.Text = rand.Next(50, 100).ToString();
            MMU.Text = rand.Next(300, 450).ToString();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            
            foreach (PieChart pie in pies) pie.Series.Clear();
            foreach (TextBox box in textBoxes) box.Text = "";
            MMU.Text = ""; 
            mmu = 0;
            sum = 0;
            info.Content = "Please insert your data";
        }

        private void Ready_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                info.Content = $"Everything went right! MMU: {MMU.Text}";
                for(int i=0;i<processes.Count(); i++) processes[i].setPages(Int32.Parse(textBoxes[i].Text));                
                this.mmu = Int32.Parse(MMU.Text);
                foreach (Process p in processes) sum += p.getPages();
                generateData();
            }
            catch (FormatException)
            {
                info.Content = "Invalid data";
            }
        }


        private void priority1_Checked(object sender, RoutedEventArgs e)
        {

            process1.setPriority(true);
        }

        private void priority2_Checked(object sender, RoutedEventArgs e)
        {
            process2.setPriority(true);
        }

        private void priority3_Checked(object sender, RoutedEventArgs e)
        {
            process3.setPriority(true);
        }

        
        private void priority4_Checked(object sender, RoutedEventArgs e)
        {
            process4.setPriority(true);
        }

        private void priority1_Unchecked(object sender, RoutedEventArgs e)
        {

            process1.setPriority(false);
        }

        private void priority2_Unchecked(object sender, RoutedEventArgs e)
        {
            process2.setPriority(false);
        }

        private void priority3_Unchecked(object sender, RoutedEventArgs e)
        {
            process3.setPriority(false);
        }

        private void priority4_Unchecked(object sender, RoutedEventArgs e)
        {
            process4.setPriority(false);
        }
    }
}
