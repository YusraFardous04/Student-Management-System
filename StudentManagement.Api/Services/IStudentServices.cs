//this file is the injection service layer
// our website has 3 layers 
//1- the controller the one in the middle that recieves http requests from the browser
//2- db(DbContext) where the data is stored
//3- service layer which is in between both of the upper layers 
//this layer has the the list of action that the controller can ask from the service layer 
// controller doesnt do the work it just askes and the service layer decides and cooordinates the action

using StudentManagement.Api.models; 
namespace StudentManagement.Api.Services
{
    public interface IStudentServices// interface is used to define a contract outlines a set of behaviors

    {
        //the interface code is a bunch of lists of method sigs meaning there is noo logic here
        List<Student> GetALL();// this list returns a list of student <this specifies >
        Student? GetById(int id);// this means when someone enters an id it might return an id or null meaning letes say someone entered #999 and there is no id like that it will bring back null \
        // this says get a student by their id 
        //Any Student Service must be able to look up a single student by their ID, and tell me either who they are OR that they don't exist."
        Student Create(Student student);// this means any student service must be able totake student object and save it as a new student 
        bool Update(int id, Student updated);// this returns a true or false thsi takes the student id i would liek to change 
       //the bool test wether the id is found or not
        bool Delete(int id);// the bool is to return wether deleting worked or not

        List<Student> Search(string query);// any student service must be able to search for student based on the text query 
        
    }

}
