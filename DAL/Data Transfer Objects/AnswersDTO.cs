﻿namespace DAL.Data_Transfer_Objects
{
    class AnswersDTO
    {
        public int AnswerID { get; set; }
        public int QQuestionID { get; set; }
        public int UserID { get; set; }
        public string AnswerText { get; set; }
    }
}
