using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.Mvc;
 
namespace OnlineAbit2013.Models
{
    public class BaseModel
    {
        public string UILanguage { get; set; }
    }

    public class HomeModel : BaseModel
    {
        public bool IsLogged { get; set; }
    }

    public enum MessageType
    {
        /// <summary>
        /// = 1 in DB
        /// </summary>
        CommonMessage = 1,
        /// <summary>
        /// = 2 in DB
        /// </summary>
        CriticalMessage = 2,
        /// <summary>
        /// = 3 in DB
        /// </summary>
        TipMessage = 3
    }

    public class PersonalMessage
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public bool IsClosed { get; set; }
        public MessageType Type { get; set; }
        public DateTime Time { get; set; }
    }

    //-------------------------------------------
    //PERSONAL INFO CLASS

    public class PersonalOffice
    {
        public string Lang { get; set; }
        public int Stage { get; set; }
        public int MaxStage { get; set; }
        public bool Enabled { get; set; }
        public int res { get; set; }
        public InfoPerson PersonInfo { get; set; }
        public PassportPerson PassportInfo { get; set; }
        // из PersonalOfficeForeign
        public VisaInfo VisaInfo { get; set; }
        public ContactsPerson ContactsInfo { get; set; }
        public EducationPerson EducationInfo { get; set; }
        public AdditionalInfoPerson AddInfo { get; set; }
        public WorkPerson WorkInfo { get; set; }
        // список файлов, типов, и тип файла
        public List<SelectListItem> FileTypes { get; set; }
        public List<AppendedFile> Files { get; set; } 
        public PersonPrivileges PrivelegeInfo { get; set; }
        public Constants ConstInfo { get; set; }
    }

    public class PersonalOfficeRecover
    {
        public string Lang { get; set; }
        public int Stage { get; set; }
        public int MaxStage { get; set; }
        public bool Enabled { get; set; }
        public InfoPerson PersonInfo { get; set; }
        public PassportPerson PassportInfo { get; set; }
        public ContactsPerson ContactsInfo { get; set; }
        public DisorderedSPBUEducation DisorderInfo { get; set; }
        public AdditionalInfoPerson AddInfo { get; set; }
    }

    public class PersonalOfficeChanging
    {
        public string Lang { get; set; }
        public int Stage { get; set; }
        public int MaxStage { get; set; }
        public bool Enabled { get; set; }
        public InfoPerson PersonInfo { get; set; }
        public PassportPerson PassportInfo { get; set; }
        public ContactsPerson ContactsInfo { get; set; }
        public CurrentSPBUEducation CurrentEducation { get; set; }
        public AdditionalInfoPerson AddInfo { get; set; }
        public ChangingAddInfo ChangingAddInfo { get; set; }
    }

    public class PersonalOfficeTransfer
    {
        public string Lang { get; set; }
        public int Stage { get; set; }
        public int MaxStage { get; set; }
        public bool Enabled { get; set; }
        public InfoPerson PersonInfo { get; set; }
        public PassportPerson PassportInfo { get; set; }
        public ContactsPerson ContactsInfo { get; set; }
        public AdditionalInfoPerson AddInfo { get; set; }
        public EducationPerson EducationInfo { get; set; }
        public CurrentEducation CurrentEducation { get; set; }
    }

    public class PersonalOfficeForeign
    {
        public string Lang { get; set; }
        public int Stage { get; set; }
        public int MaxStage { get; set; }
        public bool Enabled { get; set; }
        public InfoPerson PersonInfo { get; set; }
        public PassportPerson PassportInfo { get; set; }
        public VisaInfo VisaInfo { get; set; }
        public ContactsForeignPerson ContactsInfo { get; set; }
        public EducationPerson EducationInfo { get; set; }
        public AdditionalInfoPerson AddInfo { get; set; }
        public WorkPerson WorkInfo { get; set; }
    }

    public class PersonalOfficeAG
    {
        public string Lang { get; set; }
        public int Stage { get; set; }
        public int MaxStage { get; set; }
        public bool Enabled { get; set; }
        public InfoPerson PersonInfo { get; set; }
        public PassportPerson PassportInfo { get; set; }
        public ContactsPerson ContactsInfo { get; set; }
        public PersonSchoolInfo PersonSchoolInfo { get; set; }
        public AdditionalInfoPerson AddInfo { get; set; }
        public PersonPrivileges PrivelegeInfo { get; set; }
    }

    public class PersonalOffice_SPO
    {
        public string Lang { get; set; }
        public int Stage { get; set; }
        public int MaxStage { get; set; }
        public bool Enabled { get; set; }
        public InfoPerson PersonInfo { get; set; }
        public PassportPerson PassportInfo { get; set; }
        public ContactsPerson ContactsInfo { get; set; }
        public EducationPerson EducationInfo { get; set; }
        public AdditionalInfoPerson AddInfo { get; set; }
        public SPO_PersonPrivileges PrivelegeInfo { get; set; }
    }

    /// <summary>
    /// Личные данные - ФИО, дата, место рождения
    /// </summary>
    public class InfoPerson
    {
        public string Surname { get; set; }
        public string Name { get; set; }
        public string SecondName { get; set; }

        public string BirthPlace { get; set; }
        public string BirthDate { get; set; }

        public string Nationality { get; set; }
        public string SNILS { get; set; }

        public List<SelectListItem> NationalityList { get; set; }
        public string Sex { get; set; }
        public List<SelectListItem> SexList { get; set; }

        //public int AbitType { get; set; }
        public bool IsEqualWithRussian { get; set; }
    }
    /// <summary>
    /// Паспортные данные
    /// </summary>
    public class PassportPerson
    {
        public int AbitType { get; set; }
        public string PassportType { get; set; }
        public List<SelectListItem> PassportTypeList { get; set; }
        public string PassportSeries { get; set; }
        public string PassportNumber { get; set; }
        public string PassportAuthor { get; set; }
        public string PassportDate { get; set; }
        public string PassportCode { get; set; }
        public string PassportValid { get; set; }
    }
    /// <summary>
    /// Контактная информация
    /// </summary>
    public class ContactsPerson
    {
        public string MainPhone { get; set; }
        public string SecondPhone { get; set; }

        public string CountryId { get; set; }
        public List<SelectListItem> CountryList { get; set; }
       

        public string RegionId { get; set; }
        public List<SelectListItem> RegionList { get; set; }

        public string PostIndex { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string Korpus { get; set; }
        public string Flat { get; set; }

        public string PostIndexReal { get; set; }
        public string CityReal { get; set; }
        public string StreetReal { get; set; }
        public string HouseReal { get; set; }
        public string KorpusReal { get; set; }
        public string FlatReal { get; set; }
    }

    /// <summary>
    /// Контактная информация
    /// </summary>
    public class ContactsForeignPerson
    {
        public string MainPhone { get; set; }
        public string SecondPhone { get; set; }

        public string CountryId { get; set; }
        public List<SelectListItem> CountryList { get; set; }

        public string PostIndex { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string Korpus { get; set; }
        public string Flat { get; set; }

        public string AddressData { get; set; }
    }

    /// <summary>
    /// Данные документа об образовании
    /// </summary>
    public class EducationPerson
    {
        public string SchoolTypeId { get; set; }
        public string Barcode { get; set; }
        public List<SelectListItem> SchoolTypeList { get; set; }
        [Required]
        public string SchoolName { get; set; }

        public string AttestatRegion { get; set; }
        public string AttestatSeries { get; set; }
        public string AttestatNumber { get; set; }
        [Required]
        public string DiplomSeries { get; set; }
        [Required]
        public string DiplomNumber { get; set; }

        public string SchoolNumber { get; set; }
        public string SchoolCity { get; set; }

        public string SchoolExitYear { get; set; }
        public bool IsExcellent { get; set; }

        public bool IsEqual { get; set; }
        public string EqualityDocumentNumber { get; set; }

        public string HEEntryYear { get; set; }
        public string HEExitYear { get; set; }
        
        public string CountryEducId { get; set; }
        public string LanguageId { get; set; }

        public string ProgramName { get; set; }
        public string PersonQualification { get; set; }
        public string PersonStudyForm { get; set; }

        /// <summary>
        /// Хочу изучать анг. язык "с нуля"
        /// </summary>
        public bool StartEnglish { get; set; }
        /// <summary>
        /// Оценка по англ (если изучался)
        /// </summary>
        public string EnglishMark { get; set; }

        public List<SelectListItem> StudyFormList { get; set; }
        public List<SelectListItem> QualificationList { get; set; }
        public List<SelectListItem> CountryList { get; set; }
        public List<SelectListItem> LanguageList { get; set; }
        public List<SelectListItem> LanguageLevelList { get; set; }

        public string AvgMark { get; set; }
        public string DiplomTheme { get; set; }

        public List<EgeMarkModel> EgeMarks { get; set; }
        //public List<ForeignLanguage> Languages { get; set; }

        public bool HasTRKI { get; set; }
        public string TRKICertificateNumber { get; set; }

        public string SchoolExitClassId { get; set; }
        public List<SelectListItem> SchoolExitClassList { get; set; }

        public List<SelectListItem> RegionList { get; set; }
        public string RegionEducId { get; set; }

        public List<SelectListItem> VuzAdditionalTypeList { get; set; }
        public string VuzAdditionalTypeId { get; set; }
    }

    /// <summary>
    /// Данные о школьном образовании
    /// </summary>
    public class PersonSchoolInfo
    {
        public string SchoolTypeId { get; set; }
        public List<SelectListItem> SchoolTypeList { get; set; }
        [Required]
        public string SchoolName { get; set; }

        public string SchoolExitClassId { get; set; }
        public List<SelectListItem> SchoolExitClassList { get; set; }

        public string SchoolAddress { get; set; }

        public string AttestatRegion { get; set; }
        public string AttestatSeries { get; set; }
        public string AttestatNumber { get; set; }
        [Required]
        public string DiplomSeries { get; set; }
        [Required]
        public string DiplomNumber { get; set; }

        public string SchoolNumber { get; set; }
        public string SchoolCity { get; set; }

        public string SchoolExitYear { get; set; }
        public bool IsExcellent { get; set; }

        public bool IsEqual { get; set; }
        public string EqualityDocumentNumber { get; set; }

        public string HEEntryYear { get; set; }
        public string HEExitYear { get; set; }

        public string CountryEducId { get; set; }
        public string LanguageId { get; set; }

        public string ProgramName { get; set; }
        public string PersonQualification { get; set; }
        public string PersonStudyForm { get; set; }

        /// <summary>
        /// Хочу изучать анг. язык "с нуля"
        /// </summary>
        public bool StartEnglish { get; set; }
        /// <summary>
        /// Оценка по англ (если изучался)
        /// </summary>
        public string EnglishMark { get; set; }

        public List<SelectListItem> StudyFormList { get; set; }
        public List<SelectListItem> QualificationList { get; set; }
        public List<SelectListItem> CountryList { get; set; }
        public List<SelectListItem> LanguageList { get; set; }
        public List<SelectListItem> LanguageLevelList { get; set; }

        public string AvgMark { get; set; }
        public string DiplomTheme { get; set; }

        public List<EgeMarkModel> EgeMarks { get; set; }
        public List<ForeignLanguage> Languages { get; set; }

        public bool HasTRKI { get; set; }
        public string TRKICertificateNumber { get; set; }
    }
    
    /// <summary>
    /// Данные о месте текущего обучения (для переводящихся)
    /// </summary>
    public class CurrentEducation
    {
        public string EducationTypeId { get; set; }
        public List<SelectListItem> EducationTypeList { get; set; }
        public string EducationName { get; set; }
        public bool HasAccreditation { get; set; }
        public bool HasScholarship { get; set; }
        public string AccreditationNumber { get; set; }
        public string AccreditationDate { get; set; }
        public string SemesterId { get; set; }
        public List<SelectListItem> SemesterList { get; set; }
        public string StudyLevelId { get; set; }
        public List<SelectListItem> StudyLevelList { get; set; }
    }
    /// <summary>
    /// Сведения о том, когда отчислили
    /// </summary>
    public class DisorderedSPBUEducation
    {
        public string YearOfDisorder { get; set; }
        public string EducationProgramName { get; set; }
        public bool IsForIGA { get; set; }
    }

    public class CurrentSPBUEducation
    {
        public string StudyLevelId { get; set; }
        public List<SelectListItem> StudyLevelList { get; set; }
        public string LicenseProgramId { get; set; }
        public List<SelectListItem> LicenceProgramList { get; set; }
        public string ProfileName { get; set; }
        public string SemesterId { get; set; }
        public List<SelectListItem> SemesterList { get; set; }
    }
    public class ChangingAddInfo
    {
        public string Reason { get; set; }
    }
    public class WorkPerson
    {
        public List<SelectListItem> ScWorks { get; set; }
        public int ScWorkId { get; set; }
        public List<ScienceWorkInformation> pScWorks { get; set; }
        public List<WorkInformationModel> pWorks { get; set; }
    }

    public class ScienceWorkInformation
    {
        public Guid Id { get; set; }
        public string ScienceWorkType { get; set; }
        public string ScienceWorkInfo { get; set; }
    }
    public class WorkInformationModel
    {
        public Guid Id { get; set; }
        public string Stag { get; set; }
        public string Place { get; set; }
        public string Level { get; set; }
        public string Duties { get; set; }
    }

    public class EgeMarkModel
    {
        public Guid Id { get; set; }
        public string CertificateNum { get; set; }
        public string ExamName { get; set; }
        public string Value { get; set; }
    }

    public class AdditionalInfoPerson
    {
        public bool HostelAbit { get; set; }
        public bool HostelEduc { get; set; }
        public bool HasPrivileges { get; set; }
        public string ContactPerson { get; set; }
        public string ExtraInfo { get; set; }

        public string SocialStatus { get; set; }
        public string MaritalStatus { get; set; }

        

        public List<SelectListItem> ReturnDocumentTypeList { get; set; }
        public string ReturnDocumentTypeId { get; set; }

        [Required]
        public bool FZ_152Agree { get; set; }
    }

    public class ForeignLanguageInfo
    {
        public Guid Id { get; set; }
        public string Language { get; set; }
        public string Level { get; set; }
    }

    public class VisaInfo
    {
        public string CountryId { get; set; }
        public List<SelectListItem> CountryList { get; set; }
        public string Town { get; set; }
        public string PostAddress { get; set; }
    }

    public class PersonPrivileges
    {
        public List<SelectListItem> PrivilegesList { get; set; }
        public List<PrivilegeInformation> pPrivileges { get; set; }

        public string OlympTypeId { get; set; }
        public List<SelectListItem> OlympTypeList { get; set; }
        
        public string OlympNameId { get; set; }
        public List<SelectListItem> OlympNameList { get; set; }

        public string OlympSubjectId { get; set; }
        public List<SelectListItem> OlympSubjectList { get; set; }

        public string OlympValueId { get; set; }
        public List<SelectListItem> OlympValueList { get; set; }

        public List<OlympiadInformation> pOlympiads { get; set; }

        public string SportQualificationId { get; set; }
        public List<SelectListItem> SportQualificationList { get; set; }
        public string SportQualificationLevel { get; set; }
        public string OtherSportQualification { get; set; }
    }
    public class PrivilegeInformation
    {
        public Guid Id { get; set; }
        public string PrivilegeType { get; set; }
        public string PrivilegeInfo { get; set; }
    }
    public class OlympiadInformation
    {
        public Guid Id { get; set; }
        public string OlympType { get; set; }
        public string OlympName { get; set; }
        public string OlympSubject { get; set; }
        public string OlympValue { get; set; }
        public string DocumentSeries { get; set; }
        public string DocumentNumber { get; set; }
        public DateTime DocumentDate { get; set; }
    }
    //-------------------------------------------

    //-------------------------------------------
    //PERSON CLASS
    public class SimplePerson : BaseModel
    {
        public string Surname { get; set; }
        public string Name { get; set; }
        public string SecondName { get; set; }
        public List<SimpleApplicationPackage> Applications { get; set; }
        public List<AppendedFile> Files { get; set; }
        public List<PersonalMessage> Messages { get; set; }
    }

    public class SimpleApplication
    {
        public Guid Id { get; set; }
        public Guid EntryId { get; set; }
        public int Priority { get; set; }
        public string Profession { get; set; }
        public string ObrazProgram { get; set; }
        public string Specialization { get; set; }
        public string StudyForm { get; set; }
        public string StudyBasis { get; set; }
        public string StudyLevel { get; set; }
        public bool HasManualExams { get; set; }
        public string ManualExam { get; set; }
        public bool Enabled { get; set; }
        public bool IsGosLine { get; set; }
        public bool NeedHostel { get; set; }
        public bool HasSeparateObrazPrograms { get; set; }
        public Guid? ObrazProgramInEntryId { get; set; }
    }
    public class SimpleApplicationPackage
    {
        public Guid Id { get; set; }
        public string StudyLevel { get; set; }
        public string PriemType { get; set; }
    }

    public enum ApprovalStatus
    {
        Approved,
        Rejected,
        NotSet
    }
    public class AppendedFile
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public int FileSize { get; set; }
        public string Comment { get; set; }
        public string FileExtention { get; set; }
        public bool IsShared { get; set; }
        public string LoadDate { get; set; }
        public ApprovalStatus IsApproved { get; set; }
        public string FileType { get; set; }
    }
 
    //-------------------------------------------

    public class ApplicationModel : BaseModel
    {
        public List<SelectListItem> StudyForms { get; set; }
        public int EntryType { get; set; }
        public bool IsForeign { get; set; }

        public List<SimpleApplicationPackage> Applications { get; set; }

        public string SchoolType { get; set; }
        public int ExitClassId { get; set; }
        public int ExitClassIntValue { get; set; }
        public int VuzAddType { get; set; }
        public string StudyFormId { get; set; }

        public List<SelectListItem> StudyBasises { get; set; }
        public string StudyBasisId { get; set; }

        public string SemesterId { get; set; }
        public List<SelectListItem> SemesterList { get; set; }
    }

    //-------------------------------------------
    public class AppendFilesModel : BaseModel
    {
        public List<SelectListItem> FileTypes { get; set; }
        public List<AppendedFile> Files { get; set; }
    }

    public class ShortFileInfo
    {
        public string Path { get; set; }
        public string Id { get; set; }
        public string FileName { get; set; }
        public string FileSize { get; set; }
    }

    public class SPO_PersonPrivileges
    {
        public string Stag { get; set; }
        public string WorkPlace { get; set; }
        public string SportQualificationId { get; set; }
        public string SportQualificationLevel { get; set; }
        public string SportQualification { get; set; }
        public List<SelectListItem> SportQualificationList { get; set; }
    }

    public enum AbitType
    {
        FirstCourseBakSpec,
        Mag,
        Aspirant,
        AG,
        SPO,
        TransferBakSpec,
        TransferMag,
        RestoreBakSpec,
        RestoreMag
    }

    //-------------------------------------------
    // ./Application/Index.aspx
    public class ExtApplicationModel : BaseModel
    {
        public Guid Id { get; set; }
        public Guid CommitId { get; set; }
        public string CommitName { get; set; }
        public string Priority { get; set; }
        public string StudyForm { get; set; }
        public string StudyBasis { get; set; }
        public string Profession { get; set; }
        public string ObrazProgram { get; set; }
        public string Specialization { get; set; }
        public bool Enabled { get; set; }
        public string DateOfDisable { get; set; }
        public List<string> Exams { get; set; }
        public List<AppendedFile> Files { get; set; }
        public Guid MotivateEditId { get; set; }
        public string MotivateEditText { get; set; }

        public int EntryTypeId { get; set; }
        public int AbiturientTypeId { get; set; }
        public AbitType AbiturientType { get; set; }

        //перспективные
        public bool IsApproved { get; set; }
        public bool NotEnabled { get; set; }

        public string ComissionAddress { get; set; }
        public string ComissionYaCoord { get; set; }

        public List<KeyValuePair<int, string>> FileType { get; set; }
    }

    public class ExtApplicationCommitModel : BaseModel
    {
        public Guid Id { get; set; }

        public List<SimpleApplication> Applications { get; set; }
        public List<AppendedFile> Files { get; set; }
       
        public bool Enabled { get; set; }
        public string DateOfDisable { get; set; }

        public string StudyLevelId { get; set; }
        public int AbiturientTypeId { get; set; }

        //перспективные
        public bool IsApproved { get; set; }
        public bool NotEnabled { get; set; }

        public List<KeyValuePair<int, string>> FileType { get; set; }
    }

    //-------------------------------------------
    // ./Abiturient/MotivateMail.aspx
    public class MotivateMailModel : BaseModel
    {
        public string CommitId { get; set; }
        public string VersionId { get; set; }
        public List<SimpleApplication> Apps { get; set; }
        public List<string> lstApps { get; set; }
    }

    public enum EmailConfirmationStatus
    {
        Confirmed,
        WrongTicket,
        WrongEmail,
        FirstEmailSent
    }

    public class EmailConfirmationModel : BaseModel
    {
        public EmailConfirmationStatus RegStatus { get; set; }
        public string Email { get; set; }
        public string Link { get; set; }
    }

    public class AccountErrorModel : BaseModel
    {
        public string ErrorHtmlString { get; set; }
    }

    #region Dorms

    public class DormsModel
    {
        public int? DormsId { get; set; }
        public bool isRegistered { get; set; }
        public bool hasRightsToTimetable { get; set; }
        public DateTime? regDate { get; set; }
        public List<TimetableRow> Rows { get; set; }
        /// <summary>
        /// Юзер из Спб
        /// </summary>
        public bool isSPB { get; set; }
        /// <summary>
        /// Юзер первокурсник
        /// </summary>
        public bool isFirstCourse { get; set; }
        /// <summary>
        /// юзер находится в списке зачисленных
        /// </summary>
        public bool hasInEntered { get; set; }
    }
    public class TimetableRow
    {
        public int Hour { get; set; }
        public List<TimetableCell> Cells { get; set; }
    }
    public class TimetableCell
    {
        public int Minute { get; set; }
        public bool isLocked { get; set; }
        public int CountAbits { get; set; }
        public bool isRegistered { get; set; }
    }
    public class DataRegModel
    {
        public List<SelectListItem> listRegion { get; set; }
    }

    #endregion

    public class OpenPersonalAccountModel
    {
        public int Type { get; set; }
    }

    public class AG_ApplicationModel
    {
        public bool Enabled { get; set; }
        public string EntryClassName { get; set; }
        public int EntryClassId { get; set; }
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
        public List<SelectListItem> Professions { get; set; }
        public List<int> PersonFilesId {get; set;}
        public List<KeyValuePair<int, string>> RequiredFiles { get; set; }
        public int MaxBlocks { get; set; }
        public string CommitId { get; set; }
        public List<AG_ApplicationSipleEntity> Applications { get; set; }
    }

    public class AG_ApplicationSipleEntity
    {
        public Guid Id { get; set; }
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }
        public List<SelectListItem> ProgramList { get; set; }
        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public List<SelectListItem> ProfileList { get; set; }
        public int ManualExamId { get; set; }
        public string ManualExamName { get; set; }
        public List<SelectListItem> ManualExamList { get; set; }
    }

    public class SPO_NewApplicationModel
    {
        public bool CanChooseExitClass { get; set; }
        public string ObrazProgramId { get; set; }
        public string ObrazProgramName { get; set; }
        public List<SelectListItem> ObrazPrograms { get; set; }
        public int EntryClassId { get; set; }
        public List<SelectListItem> Professions { get; set; }
    }

    public class NewApplicationRectorScholarshipModel
    {
        public string Message { get; set; }
        public List<AppendedFile> Files { get; set; }
    }

    public class Mag_ApplicationModel
    {
        public bool Enabled { get; set; } // доступно?
        public int MaxBlocks { get; set; } // макс количество блоков
        public string CommitId { get; set; } // коммит
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
        public int SemestrId { get; set; }
        public List<SelectListItem> StudyFormList { get; set; }
        public List<SelectListItem> StudyBasisList { get; set; }
        public List<SelectListItem> SemestrList { get; set; }
        public List<SelectListItem> FacultyList { get; set; }
        public List<SelectListItem> StudyLevelGroupList { get; set; }
        public List<Mag_ApplicationSipleEntity> Applications { get; set; }

        public List<KeyValuePair<int, string>> RequiredFiles { get; set; }
    }

    public class Mag_ApplicationSipleEntity
    {
        public Guid Id { get; set; }
        public int StudyFormId { get; set; }
        public string StudyFormName { get; set; }
        public int StudyBasisId { get; set; }
        public string StudyBasisName { get; set; }
        public bool IsReduced { get; set; }
        public bool IsParallel { get; set; }
        public bool IsSecond { get; set; }
        public int ProfessionId { get; set; }
        public string ProfessionName { get; set; }
        public int ObrazProgramId { get; set; }
        public string ObrazProgramName { get; set; }
        public Guid SpecializationId { get; set; }
        public string SpecializationName { get; set; }
        public List<SelectListItem> ProfessionList { get; set; }
        public List<SelectListItem> ObrazProgramList { get; set; }
        public List<SelectListItem> SpecializationList { get; set; }
        public int FacultyId { get; set; }
        public string FacultyName { get; set; }
        public bool Hostel { get; set; }
        public int StudyLevelGroupId { get; set; }
        public string StudyLevelGroupName { get; set; }
        public bool? IsGosLine { get; set; }
    }

    public class SPO_ApplicationModel
    {
        public bool Enabled { get; set; } // доступно?
        public int MaxBlocks { get; set; } // макс количество блоков
        public string CommitId { get; set; } // коммит
        public bool can11kl { get; set; } //можно ли подавать на 11 классов

        public List<SelectListItem> StudyFormList { get; set; }
        public List<SelectListItem> StudyBasisList { get; set; }

        public List<SelectListItem> FacultyList { get; set; }

        public List<SPO_ApplicationSipleEntity> Applications { get; set; }

        public List<KeyValuePair<int, string>> RequiredFiles { get; set; }
    }

    public class SPO_ApplicationSipleEntity
    {
        public Guid Id { get; set; }
        public int StudyFormId { get; set; }
        public string StudyFormName { get; set; }
        public int StudyBasisId { get; set; }
        public string StudyBasisName { get; set; }
        public bool IsReduced { get; set; }
        public bool IsParallel { get; set; }
        public bool IsSecond { get; set; }
        public int ProfessionId { get; set; }
        public string ProfessionName { get; set; }
        public int ObrazProgramId { get; set; }
        public string ObrazProgramName { get; set; }
        public Guid SpecializationId { get; set; }
        public string SpecializationName { get; set; }
        public List<SelectListItem> ProfessionList { get; set; }
        public List<SelectListItem> ObrazProgramList { get; set; }
        public List<SelectListItem> SpecializationList { get; set; }
        public int FacultyId { get; set; }
        public string FacultyName { get; set; }
        public bool Hostel { get; set; }
    }

    public class Constants
    {
        public int? Surname { get; set; }
        public int? Name { get; set; }
        public int? SecondName { get; set; }
        public int? BirthPlace { get; set; } 
        public int? PassportAuthor { get; set; }
        public int? Parents { get; set; }
        public int? AddInfo { get; set; } 
    }

    public class StandartObrazProgramInEntryRow
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool HasProfiles { get; set; }
        public int Priority { get; set; }
        public int DefaultPriority { get; set; }
    }
}