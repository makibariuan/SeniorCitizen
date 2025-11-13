<template>
  <div class="layout pds">
    <LeftMenu />
    <Header title="Personal Data Sheet"/>
    <div class="content">
      <div class="w-[90%] mx-auto">
        <!-- <h1 class="text-2xl font-bold mb-4">Personal Data Sheet</h1> -->

        <!-- Folder-style Tabs -->
        <div class="tab-container">
          <button v-for="tab in tabs"
                  :key="tab"
                  @click="activeTab = tab"
                  class="tab-btn"
                  :class="{ active: activeTab === tab }">
            {{ tab }}
          </button>
        </div>

        <!-- Folder-like Content Area -->
        <div class="tab-wrapper">
          <div class="tab-content">
            <transition name="fade-slide" mode="out-in">
              <div :key="activeTab" class="form-wrapper">
                <!-- Personal Data Sheet 1 -->
                
                <div v-if="activeTab === 'Personal Data Sheet 1'">
                  <h2 class="sub-title">Personal Information</h2>
                  <div class="pds-table-wrapper">
                    <!-- <h2 class="sub-title">Personal Information</h2> -->
                    <table class="pds-table" >
                      <tbody>
                        <!--<tr>
                          <td>
                            <input v-model="user.csidNo" placeholder="CSID No" class="input" />
                          </td>
                        </tr>-->
                        <tr>
                          <td>
                            <p class="auth-input-label-bold">Agency Employee No.</p>
                            <input v-model="user.agencyEmployeeNo" placeholder="Agency Employee No" class="auth-input" readonly />
                          </td>

                          <td>
                            <p class="auth-input-label-bold">Department*</p>
                            <select v-model="user.departmentID" class="auth-input">
                              <option disabled value="">-- Select Department --</option>
                              <option v-for="d in departments" :key="d.departmentID" :value="d.departmentID">
                                {{ d.departmentName }}
                              </option>
                            </select>
                          </td>
                          <td>
                            <p class="auth-input-label-bold">Designation*</p>
                            <input v-model="user.designation" placeholder="Designation" class="auth-input" />
                          </td>
                        </tr>
                        <tr>
                          <td>
                            <p class="auth-input-label-bold">Surname*</p>
                            <input v-model="user.surname" placeholder="Surname" class="auth-input" readonly />
                          </td>
                          <td>
                            <p class="auth-input-label-bold">First Name*</p>
                            <input v-model="user.firstName" placeholder="First Name" class="auth-input" readonly />
                          </td>
                          <td>
                            <p class="auth-input-label-bold">Middle Name*</p>
                            <input v-model="user.middleName" placeholder="Middle Name" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label-bold">Name Extension*</p>
                            <input v-model="user.nameExtension" placeholder="Name Extension (e.g., Jr., III)" class="auth-input" />
                          </td>
                        </tr>
                        <tr>
                          <td>
                            <p class="auth-input-label-bold">Date of Birth*</p>
                            <input v-model="user.dateOfBirth" type="date" class="auth-input" readonly />
                          </td>
                          <td>
                            <p class="auth-input-label-bold">Place of Birth*</p>
                            <input v-model="user.placeOfBirth" placeholder="Place of Birth" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label-bold">Blood Type*</p>
                            <input v-model="user.bloodType" placeholder="Bloodtype" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label-bold">Citizenship*</p>
                            <input v-model="user.citizenship" placeholder="Citizenship" class="auth-input" />
                          </td>
                        </tr>
                      </tbody>
                    </table>

                    <!-- UPLOAD BIRTH CERTIFICATE -->
                    <!--<div class="upload-file-btn" @click="">
                      <img src="../assets/upload.png"/>
                      Upload  PSA Birth Certificate
                    </div>-->
                    
                    <table class="pds-table">
                      <tbody>
                        <tr>
                          <td>
                            <p class="auth-input-label-bold">Civil Status*</p>
                            <select v-model="user.civilStatusID" class="auth-input">
                               <!-- Note:
                                  Single
                                  Married
                                  Divorced
                                  Widowed
                                  Separated 
                                  Other      -->
                              <option disabled value="">-- Select Civil Status --</option>
                              <option v-for="cs in civilStatuses" :key="cs.civilStatusID" :value="cs.civilStatusID">
                                {{ cs.statusName }}
                              </option>
                            </select>
                          </td>
                          <td colspan="3">
                            <div class="note">
                              <span>
                                Required Documents to Upload<br>
                                • Married - PSA Marriage Certificate<br>
                                • Widowed - PSA Death / PSA Marriage Certificate<br>
                                • Divorced - PSA Marriage Certificate with an annotation of divorced / PSA Report of Marriage annotation of divorced<br>
                                • Legally Separated - PSA Marriage Certificate with a court order of legal separation / PSA Report of Marriage with a court order of legal separation
                              </span>
                            </div>
                          </td>
                        </tr>
                      </tbody>
                    </table>

                    <!-- UPLOAD REQUIRED DOCUMENT -->
                    <!--<div class="upload-file-btn" @click="">
                      <img src="../assets/upload.png"/>
                      Upload  Required Document
                    </div>-->


                    <table class="pds-table">
                      <tbody>
                        <tr>
                          <td>
                            <p class="auth-input-label-bold">Sex</p>
                            <select v-model="user.sexID" class="auth-input">
                              <option disabled value="">-- Select Gender --</option>
                              <option v-for="g in genders" :key="g.genderID" :value="g.genderID">
                                {{ g.genderName }}
                              </option>
                            </select>
                          </td>
                          <td>
                            <p class="auth-input-label-bold">Height (m)*</p>
                            <input v-model="user.heightCM" type="number" step="0.01" placeholder="Height (cm)" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label-bold">Weight (kg)*</p>
                            <input v-model="user.weightKG" type="number" step="0.01" placeholder="Weight (kg)" class="auth-input" />
                          </td>
                          <!-- <td style="visibility: hidden">            SPACER  
                            <p class="auth-input-label-bold"></p>    EMPTY  
                            <input class="input" disabled/>          TB DATA 
                          </td> -->
                        </tr>
                        <tr>
                          <td>
                            <p class="auth-input-label-bold">GSIS ID No.</p>
                            <input v-model="user.gsisID" placeholder="GSIS ID" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label-bold">PAG-IBIG ID No.</p>
                            <input v-model="user.pagibigID" placeholder="PAG-IBIG ID" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label-bold">PHILHEALTH ID No.</p>
                            <input v-model="user.philhealthID" placeholder="PhilHealth ID" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label-bold">SSS ID No.</p>
                            <input v-model="user.sssNo" placeholder="SSS No" class="auth-input" />
                          </td>
                        </tr>
                        <tr>
                          <td>
                            <p class="auth-input-label-bold">TIN No.</p>
                            <input v-model="user.tin" placeholder="TIN" class="auth-input" />
                          </td>
                          <td>
                          </td>
                        </tr>
                        <tr>
                          <td colspan="3">
                            <p class="auth-input-label-bold">Residential Address*</p>
                            <input v-model="user.residentialAddress" placeholder="Residential Address" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label-bold">ZIP Code*</p>
                            <input v-model="user.residentialZip" placeholder="Residential Zip" class="auth-input" />
                          </td>
                        </tr>
                        <tr>
                          <td colspan="3">
                            <span class="auth-input-label-bold">
                              Permanent Address*
                              <label class="auth-input-label-bold" style="margin-left:0">
                                <input type="checkbox" v-model="user.isSameAddress" style="justify-self: left;">
                                Same with residential address
                              </label>
                            </span>
                                               <!-- TO FIX  -->
                            <input v-model="user.permanentAddress" placeholder="Permanent Address" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label-bold">ZIP Code*</p>
                            <input v-model="user.permanentZip" placeholder="Permanent Zip" class="auth-input" />
                          </td>
                        </tr>
                        <tr>
                          <td>
                            <p class="auth-input-label-bold">Telephone No.*</p>
                            <input v-model="user.telephoneNo" placeholder="Telephone No" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label-bold">Mobile No.*</p>
                            <input v-model="user.mobileNo" placeholder="Mobile No" class="auth-input" />
                          </td>
                          <td colspan="2">
                            <p class="auth-input-label-bold">Email Address</p>
                            <input v-model="user.email" placeholder="Email" type="email" class="auth-input" disabled/>
                          </td>
                        </tr>
                      </tbody>
                    </table>
                  </div>

                  <h2 class="sub-title">Family Background</h2>
                  <div class="pds-table-wrapper">
                    <table class="pds-table">
                      <tbody>
                        <tr>
                          <td>
                            <p class="auth-input-label-bold">Spouse's Surname*</p>
                            <input v-model="user.spouseSurname" placeholder="Spouse Surname" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label-bold">Spouse's First Name*</p>
                            <input v-model="user.spouseFirstName" placeholder="Spouse First Name" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label-bold">Spouse's Middle Name*</p>
                            <input v-model="user.spouseMiddleName" placeholder="Spouse Middle Name" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label-bold">Spouse's Name Extension*</p>
                            <input v-model="user.spouseNameExtension" placeholder="Spouse Name Extension" class="auth-input" />
                          </td>
                        </tr>
                        <tr>
                          <td>
                            <p class="auth-input-label-bold">Spouse's Occupation*</p>
                            <input v-model="user.spouseOccupation" placeholder="Spouse Occupation" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label-bold">Spouse's Telephone No.*</p>
                            <input v-model="user.spouseTelephone" placeholder="Spouse Telephone" class="auth-input" />
                          </td>
                        </tr>
                        <tr>
                          <td colspan="2">
                            <p class="auth-input-label-bold">Employer / Business Name*</p>
                            <input v-model="user.spouseEmployer" placeholder="Spouse Employer" class="auth-input" />
                          </td>
                          <td colspan="2">
                            <p class="auth-input-label-bold">Business Address*</p>
                            <input v-model="user.spouseBusinessAddress" placeholder="Spouse Business Address" class="auth-input" />
                          </td>
                        </tr>
                        
                        <!-- ADD CHILDREN -->
                         <tr>
                          <td colspan="2">
                            <p class="auth-input-label-bold">Name of Children (Write in full name and list all)*</p>
                          </td>
                          <td colspan="2">
                            <p class="auth-input-label-bold">Date of Birth*</p>
                          </td>
                         </tr>
                        <tr v-if="user.children.length" v-for="(child, idx) in user.children" :key="idx">
                          <td colspan="2">
                            <input v-model="child.fullName" placeholder="Full Name" class="auth-input" />
                          </td>
                          <td>
                            <input v-model="child.dateOfBirth" type="date" class="auth-input" />
                          </td>
                          <td>
                            <!-- Remove Button for Children -->
                            <button @click="user.children.splice(idx, 1)" class="btn-white">Remove</button>
                          </td>
                        </tr>
                        <tr>
                          <td colspan="2">
                            <input v-model="user.spouseEmployer" placeholder="" class="auth-input" disabled/>
                          </td>
                          <td>
                            <input placeholder="" class="auth-input" disabled/>
                          </td>
                          <td>
                            <button @click="user.children.push({ fullName: '', dateOfBirth: '' })" class="btn-white">Add +</button>
                          </td>
                        </tr>
                        <tr>
                          <td>
                            <p class="auth-input-label-bold">Father's Surname*</p>
                            <input v-model="user.fatherSurname" placeholder="Father Surname" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label-bold">Father's First Name*</p>
                            <input v-model="user.fatherFirstName" placeholder="Father First Name" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label-bold">Father's Middle Name*</p>
                            <input v-model="user.fatherMiddleName" placeholder="Father Middle Name" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label-bold">Father's Name Extension*</p>
                            <input v-model="user.fatherNameExtension" placeholder="Father Name Extension" class="auth-input" />
                          </td>
                        </tr>
                        <tr>
                          <td>
                            <p class="auth-input-label-bold">Mother's Surname*</p>
                            <input v-model="user.motherSurname" placeholder="Mother Surname" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label-bold">Mother's First Name*</p>
                            <input v-model="user.motherFirstName" placeholder="Mother First Name" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label-bold">Mother's Middle Name*</p>
                            <input v-model="user.motherMiddleName" placeholder="Mother Middle Name" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label-bold">Mother's Name Extension*</p>
                            <input v-model="user.motherNameExtension" placeholder="Mother Name Extension" class="auth-input" />
                          </td>
                        </tr>
                      </tbody>
                    </table>
                  </div>

                    <!-- - - - EDUCATIONAL BACKGROUND - - - -->
                  <h2 class="sub-title">Educational Background</h2>
                  <div class="pds-table-wrapper">
                    <table class="pds-table">
                      <tbody>
                        <tr>
                                <!-- - - - ELEMENTARY - - - -->
                          <td><p class="auth-input-label-bold" style="margin-bottom:10px">Elementary</p></td>
                        </tr>
                        <tr>
                          <td colspan="2">
                            <p class="auth-input-label">Name of School*</p>
                            <input v-model="user.elementaryName" placeholder="Elementary School" class="auth-input" />
                          </td>
                          <td colspan="2">
                            <p class="auth-input-label">Basic Education/Degree/Course (Write in full)*</p>
                            <input v-model="user.elementaryType" placeholder="Elementary School" class="auth-input" />
                          </td>
                        </tr>
                        <tr>
                          <td>
                            <p class="auth-input-label">Period of Attendance (Start)*</p>
                            <input v-model="user.elementaryStart" placeholder="Elementary School" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label">Period of Attendance (End)*</p>
                            <input v-model="user.elementaryEnd" placeholder="Elementary School" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label">Year Graduated*</p>
                            <input v-model="user.elementaryGraduated" placeholder="Elementary School" class="auth-input" />
                          </td>
                              <!-- SPACER EMPTY DATA -->
                          <td style="visibility: hidden">
                            <p class="auth-input-label-bold"></p>
                            <input class="input" disabled/>
                          </td>
                        </tr>

                        <tr>
                                <!-- - - - SECONDARY - - - -->
                          <td><p class="auth-input-label-bold" style="margin-bottom:10px">Secondary</p></td>
                        </tr>
                        <tr>
                          <td colspan="2">
                            <p class="auth-input-label">Name of School*</p>
                            <input v-model="user.secondaryName" placeholder="Secondary School" class="auth-input" />
                          </td>
                          <td colspan="2">
                            <p class="auth-input-label">Basic Education/Degree/Course (Write in full)*</p>
                            <input v-model="user.secondaryType" placeholder="secondary School" class="auth-input" />
                          </td>
                        </tr>
                        <tr>
                          <td>
                            <p class="auth-input-label">Period of Attendance (Start)*</p>
                            <input v-model="user.secondaryStart" placeholder="secondary School" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label">Period of Attendance (End)*</p>
                            <input v-model="user.secondaryEnd" placeholder="secondary School" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label">Year Graduated*</p>
                            <input v-model="user.secondaryGraduated" placeholder="secondary School" class="auth-input" />
                          </td>
                              <!-- SPACER EMPTY DATA -->
                          <td style="visibility: hidden">
                            <p class="auth-input-label-bold"></p>
                            <input class="input" disabled/>
                          </td>
                        </tr>

                        <tr>
                                <!-- - - - VOCATIONAL - - - -->
                          <td><p class="auth-input-label-bold" style="margin-bottom:10px">Vocational / Trade Course</p></td>
                        </tr>
                        <tr>
                          <td colspan="2">
                            <p class="auth-input-label">Name of School*</p>
                            <input v-model="user.vocationalName" placeholder="vocational School" class="auth-input" />
                          </td>
                          <td colspan="2">
                            <p class="auth-input-label">Basic Education/Degree/Course (Write in full)*</p>
                            <input v-model="user.vocationalType" placeholder="vocational School" class="auth-input" />
                          </td>
                        </tr>
                        <tr>
                          <td>
                            <p class="auth-input-label">Period of Attendance (Start)*</p>
                            <input v-model="user.vocationalStart" placeholder="vocational School" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label">Period of Attendance (End)*</p>
                            <input v-model="user.vocationalEnd" placeholder="vocational School" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label">Year Graduated*</p>
                            <input v-model="user.vocationalGraduated" placeholder="vocational School" class="auth-input" />
                          </td>
                              <!-- SPACER EMPTY DATA -->
                          <td style="visibility: hidden">
                            <p class="auth-input-label-bold"></p>
                            <input class="input" disabled/>
                          </td>
                        </tr>

                        <tr>
                                <!-- - - - COLLEGE - - - -->
                          <td><p class="auth-input-label-bold" style="margin-bottom:10px">College</p></td>
                        </tr>
                        <tr>
                          <td colspan="2">
                            <p class="auth-input-label">Name of School*</p>
                            <input v-model="user.collegeName" placeholder="college School" class="auth-input" />
                          </td>
                          <td colspan="2">
                            <p class="auth-input-label">Basic Education/Degree/Course (Write in full)*</p>
                            <input v-model="user.collegeType" placeholder="college School" class="auth-input" />
                          </td>
                        </tr>
                        <tr>
                          <td>
                            <p class="auth-input-label">Period of Attendance (Start)*</p>
                            <input v-model="user.collegeStart" placeholder="college School" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label">Period of Attendance (End)*</p>
                            <input v-model="user.collegeEnd" placeholder="college School" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label">Year Graduated*</p>
                            <input v-model="user.collegeGraduated" placeholder="college School" class="auth-input" />
                          </td>
                              <!-- SPACER EMPTY DATA -->
                          <td style="visibility: hidden">
                            <p class="auth-input-label-bold"></p>
                            <input class="input" disabled/>
                          </td>
                        </tr>

                        <tr>
                                <!-- - - - GRADUATE STUDIES - - - -->
                          <td><p class="auth-input-label-bold" style="margin-bottom:10px">Graduate Studies</p></td>
                        </tr>
                        <tr>
                          <td colspan="2">
                            <p class="auth-input-label">Name of School*</p>
                            <input v-model="user.graduateName" placeholder="graduate School" class="auth-input" />
                          </td>
                          <td colspan="2">
                            <p class="auth-input-label">Basic Education/Degree/Course (Write in full)*</p>
                            <input v-model="user.graduateType" placeholder="graduate School" class="auth-input" />
                          </td>
                        </tr>
                        <tr>
                          <td>
                            <p class="auth-input-label">Period of Attendance (Start)*</p>
                            <input v-model="user.graduateStart" placeholder="graduate School" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label">Period of Attendance (End)*</p>
                            <input v-model="user.graduateEnd" placeholder="graduate School" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label">Year Graduated*</p>
                            <input v-model="user.graduateGraduated" placeholder="graduate School" class="auth-input" />
                          </td>
                              <!-- SPACER EMPTY DATA -->
                          <td style="visibility: hidden">
                            <p class="auth-input-label-bold"></p>
                            <input class="input" disabled/>
                          </td>
                        </tr>

                        <tr>
                          <td>
                            <button @click="" class="btn-white" style="width: 100%">Go Back</button>
                          </td>
                          <td style="visibility: hidden">
                            <p class="auth-input-label-bold"></p>
                            <input class="input" disabled/>
                          </td>
                          <td>
                            <button @click="save" class="btn-white" style="width: 100%">Save</button>
                          </td>
                          <td>
                            <button @click="activeTab = 'Personal Data Sheet 2'" class="btn" style="width: 100%">Next</button>
                          </td>
                        </tr>

                      </tbody>
                    </table>

                  </div>
                </div>



                
                <!-- Personal Data Sheet 2 -->
                <div v-else-if="activeTab === 'Personal Data Sheet 2'">
                  <div class="form-wrapper">

                            <!-- CIVIL SERVICE ELIGIBILITY -->
                    <h2 class="sub-title">Civil Service Eligibility</h2>
                    <div class="pds-table-wrapper">

                      <!-- <h2 class="sub-title">Civil Service Eligibility</h2> -->

                      <table class="pds-table">
                        <tbody>
                          <tr>
                            <td colspan="2">
                              <p class="auth-input-label-bold">Career Service/RA 1080 (Board/Bar) Under Special Laws/CES/CSEE/Barangary Eligibility/Driver's License*</p>
                            </td>
                            <td>
                              <p class="auth-input-label-bold">Rating*</p>
                            </td>
                            <td>
                              <p class="auth-input-label-bold">Date of Examination/Conferment*</p>
                            </td>
                          </tr>
                          <tr>
                            <td colspan="2">
                              <input placeholder="" class="auth-input" disabled/>
                            </td>
                            <td>
                              <input placeholder="" class="auth-input" disabled/>
                            </td>
                            <td>
                              <input placeholder="" class="auth-input" disabled/>
                            </td>
                          </tr>
                        </tbody>
                      </table>

                      <table class="pds-table">
                        <thead>
                          <tr>
                            <th>Career Service</th>
                            <th>Rating</th>
                            <th>Date of Examination</th>
                            <th>Place of Examination</th>
                            <th>License Number</th>
                            <th>License Validity</th>
                            <th>Date Released</th>
                            <th>Action</th>
                          </tr>
                        </thead>
                        <tbody>
                          <tr v-for="(eligibility, idx) in user.civilServiceEligibilities" :key="idx">
                            <td><input v-model="eligibility.careerService" placeholder="Career Service" /></td>
                            <td><input v-model="eligibility.rating" placeholder="Rating" /></td>
                            <td><input v-model="eligibility.dateOfExamination" type="date" /></td>
                            <td><input v-model="eligibility.placeOfExamination" placeholder="Place of Exam" /></td>
                            <td><input v-model="eligibility.licenseNumber" placeholder="License No." /></td>
                            <td><input v-model="eligibility.licenseValidity" type="date" /></td>
                            <td><input v-model="eligibility.dateReleased" type="date" /></td>
                            <td>
                              <button @click="deleteCivilServiceEligibility(idx)" class="delete-btn">Delete</button>
                            </td>
                          </tr>
                        </tbody>
                      </table>



                    </div>
                    <button @click="user.civilServiceEligibilities.push({ careerService: '', rating: '', dateOfExamination: '', placeOfExamination: '', licenseNumber: '', licenseValidity: '', dateReleased: '' })"
                            class="btn">
                      Add Eligibility
                    </button>

                    <h2 class="text-xl font-semibold mb-2">Work Experiences</h2>
                    <div class="form-wrapper">
                      <div class="pds-table-wrapper">
                        <table class="pds-table">
                          <thead>
                            <tr>
                              <th>Date From</th>
                              <th>Date To</th>
                              <th>Position Title</th>
                              <th>Department / Agency / Company</th>
                              <th>Monthly Salary</th>
                              <th>Salary Grade & Step</th>
                              <th>Status of Appointment</th>
                              <th>Gov’t Service</th>
                              <th>Description</th>
                              <th>Action</th>
                            </tr>
                          </thead>
                          <tbody>
                            <tr v-for="(work, idx) in user.workExperiences" :key="idx">
                              <td><input v-model="work.dateFrom" type="date" /></td>
                              <td><input v-model="work.dateTo" type="date" /></td>
                              <td><input v-model="work.positionTitle" placeholder="Position Title" /></td>
                              <td><input v-model="work.departmentAgencyCompany" placeholder="Department / Agency / Company" /></td>
                              <td><input v-model="work.monthlySalary" type="number" step="0.01" placeholder="Salary" /></td>
                              <td><input v-model="work.salaryGradeStep" placeholder="SG-Step" /></td>
                              <td><input v-model="work.statusOfAppointment" placeholder="Status" /></td>
                              <td>
                                <input type="checkbox" v-model="work.isGovernmentService" />
                              </td>
                              <td><input v-model="work.description" placeholder="Description" /></td>
                              <td>
                                <button @click="deleteWorkExperience(idx)" class="delete-btn">Delete</button>
                              </td>
                            </tr>
                          </tbody>
                        </table>
                      </div>

                      <button @click="user.workExperiences.push({
                          dateFrom: '',
                          dateTo: '',
                          positionTitle: '',
                          departmentAgencyCompany: '',
                          monthlySalary: '',
                          salaryGradeStep: '',
                          statusOfAppointment: '',
                          isGovernmentService: false,
                          description: ''
                        })"
                              class="btn">
                        Add Work Experience
                      </button>
                    </div>
                    <button @click="save" class="btn">Save</button>
                  </div>
                </div>

                <!-- Personal Data Sheet 3 -->
                <div v-else-if="activeTab === 'Personal Data Sheet 3'">
                  <h2 class="sub-title">Voluntary Work</h2>
                  <div class="form-wrapper">
                    <div class="pds-table-wrapper">
                      <table class="pds-table">
                        <thead>
                          <tr>
                            <th>Organization</th>
                            <th>Date From</th>
                            <th>Date To</th>
                            <th>Hours</th>
                            <th>Role</th>
                            <th>Action</th> <!-- ✅ new column -->
                          </tr>
                        </thead>
                        <tbody>
                          <tr v-for="(work, idx) in user.voluntaryWorks" :key="idx">
                            <td><input v-model="work.organization" placeholder="Organization" /></td>
                            <td><input v-model="work.dateFrom" type="date" /></td>
                            <td><input v-model="work.dateTo" type="date" /></td>
                            <td><input v-model="work.numberOfHours" type="number" placeholder="Hours" /></td>
                            <td><input v-model="work.position" placeholder="Role" /></td>
                            <td>
                              <button @click="deleteVoluntaryWork(idx)" class="delete-btn">Delete</button>
                            </td>
                          </tr>
                        </tbody>
                      </table>
                    </div>
                    <button @click="user.voluntaryWorks.push({ organization: '', dateFrom: '', dateTo: '', numberOfHours: '', position: '' })" class="btn">Add Voluntary Work</button>

                    <h2 class="text-xl font-semibold mb-2">Learning and Development</h2>
                    <div class="pds-table-wrapper">
                      <table class="pds-table">
                        <thead>
                          <tr>
                            <th>Title</th>
                            <th>Date From</th>
                            <th>Date To</th>
                            <th>Hours</th>
                            <th>Type of LD</th>
                            <th>Conducted By</th>
                            <th>Action</th> <!-- ✅ new column -->
                          </tr>
                        </thead>
                        <tbody>
                          <tr v-for="(t, idx) in user.trainings" :key="idx">
                            <td><input v-model="t.title" class="input" placeholder="Title" /></td>
                            <td><input v-model="t.dateFrom" type="date" class="input" /></td>
                            <td><input v-model="t.dateTo" type="date" class="input" /></td>
                            <td><input v-model="t.numberOfHours" type="number" class="input" placeholder="Hours" /></td>
                            <td><input v-model="t.typeOfLD" class="input" placeholder="Type of LD" /></td>
                            <td><input v-model="t.conductedBy" class="input" placeholder="Conducted by" /></td>
                            <td>
                              <button @click="deleteTraining(idx)" class="delete-btn">Delete</button>
                            </td>
                          </tr>
                        </tbody>
                      </table>
                    </div>
                    <button @click="user.trainings.push({ title: '', dateFrom: '', dateTo: '', numberOfHours: '', typeOfLD: '', conductedBy: '' })" class="btn">Add Training</button>

                    <h2 class="text-xl font-semibold mb-2">Other Information</h2>
                    <h3 class="text-xl font-semibold mb-2">Special Skills/Hobbies</h3>
                    <div class="pds-table-wrapper">
                      <table class="pds-table">
                        <thead>
                          <tr>
                            <th>Skills/Hobbies</th>
                            <th>Action</th> <!-- ✅ new column -->
                          </tr>
                        </thead>
                        <tbody>
                          <tr v-for="(sh, idx) in user.skillsHobbies" :key="idx">
                            <td><input v-model="sh.skillOrHobby" placeholder="Skill or Hobby" class="input" /></td>
                            <td>
                              <button @click="deleteSkillOrHobby(idx)" class="delete-btn">Delete</button>
                            </td>
                          </tr>
                        </tbody>
                      </table>
                    </div>
                    <button @click="user.skillsHobbies.push({ skillOrHobby: '' })" class="btn">Add Skill/Hobby</button>

                    <h3 class="text-xl font-semibold mb-2">Distinction</h3>
                    <div class="pds-table-wrapper">
                      <table class="pds-table">
                        <thead>
                          <tr>
                            <th>Distinctions</th>
                            <th>Action</th> <!-- ✅ new column -->
                          </tr>
                        </thead>
                        <tbody>
                          <tr v-for="(sh, idx) in user.distinctions" :key="idx">
                            <td><input v-model="sh.distinction" placeholder="Distinction" class="input" /></td>
                            <td>
                              <button @click="deleteDistinction(idx)" class="delete-btn">Delete</button>
                            </td>
                          </tr>
                        </tbody>
                      </table>
                    </div>
                    <button @click="user.distinctions.push({ distinction: '' })" class="btn">Add Distinction</button>

                    <h3 class="text-xl font-semibold mb-2">Membership</h3>
                    <div class="pds-table-wrapper">
                      <table class="pds-table">
                        <thead>
                          <tr>
                            <th>Membership</th>
                            <th>Action</th> <!-- ✅ new column -->
                          </tr>
                        </thead>
                        <tbody>
                          <tr v-for="(sh, idx) in user.memberships" :key="idx">
                            <td><input v-model="sh.organizationName" placeholder="Organization" class="input" /></td>
                            <td>
                              <button @click="deleteMemberShip(idx)" class="delete-btn">Delete</button>
                            </td>
                          </tr>
                        </tbody>
                      </table>
                    </div>
                    <button @click="user.memberships.push({ organizationName: '' })" class="btn">Add Organization</button>
                    <button @click="save" class="btn">Save</button>

                  </div>
                </div>

                <!-- Personal Data Sheet 4 -->
                <div v-else-if="activeTab === 'Personal Data Sheet 4'">
                  <h2 class="sub-title">Other Information</h2>
                  <div v-for="(info, idx) in c4.other" :key="idx" class="border p-2 mb-2">
                    <input v-model="info.skill" placeholder="Skill" class="input" />
                    <input v-model="info.recognition" placeholder="Recognition" class="input" />
                  </div>
                  <button @click="addOther" class="btn">Add Info</button>
                  <button @click="saveOther" class="btn">Save</button>

                  <h2 class="text-xl font-semibold mt-6">References</h2>
                  <div v-for="(ref, idx) in c4.references" :key="idx" class="border p-2 mb-2">
                    <input v-model="ref.fullName" placeholder="Full Name" class="input" />
                    <input v-model="ref.address" placeholder="Address" class="input" />
                    <input v-model="ref.telephone" placeholder="Telephone" class="input" />
                  </div>
                  <button @click="addRef" class="btn">Add Reference</button>
                  <button @click="save" class="btn">Save</button>
                </div>
              </div>
            </transition>
          </div>
        </div>
      </div>
    </div>
  </div>

  <!-- Reusable dialog -->
  <DialogBox :show="showDialog"
             :title="dialogTitle"
             :message="dialogMessage"
             @close="showDialog = false" />

  <!-- Loading Dialog (always on top of everything) -->
  <LoadingDialog :visible="isLoading" />
</template>


<script setup>
  import LeftMenu from './LeftMenu.vue'
  import Header from './Header.vue'
  import api from "@/api";
  import { ref, onMounted, computed } from "vue";
  import { useAuthStore } from "@/stores/auth";
  import DialogBox from "@/components/DialogBox.vue";
  import LoadingDialog from "./LoadingDialog.vue";

  const showDialog = ref(false);
  const dialogTitle = ref("");
  const dialogMessage = ref("");
  const isLoading = ref(false);

  const tabs = ["Personal Data Sheet 1", "Personal Data Sheet 2", "Personal Data Sheet 3", "Personal Data Sheet 4"];
  const activeTab = ref("Personal Data Sheet 1");

  const auth = useAuthStore();
  const personId = computed(() => auth.userId); // logged in user’s ID
  const personEmail = computed(() => auth.email);
  const birthdate = computed(() => auth.birthdate);
  const employeeID = computed(() => auth.employeeID);
  const firstName = computed(() => auth.firstName);
  const lastName = computed(() => auth.lastName);

  const defaultEducation = [
    { degree: "Elementary", schoolName: "", attendanceFrom: null, attendanceTo: null },
    { degree: "Secondary", schoolName: "", attendanceFrom: null, attendanceTo: null },
    { degree: "Vocational/Trade Course", schoolName: "", attendanceFrom: null, attendanceTo: null },
    { degree: "College", schoolName: "", attendanceFrom: null, attendanceTo: null },
    { degree: "Graduate Studies", schoolName: "", attendanceFrom: null, attendanceTo: null },
  ];

  // ------------------ State ------------------
  const user = ref({
    personID: personId.value,
    csidNo: "",
    surname: lastName.value,
    firstName: firstName.value,
    middleName: "",
    nameExtension: "",
    dateOfBirth: birthdate.value,
    placeOfBirth: "",
    bloodType: "",
    nationality: "",
    civilStatusID: null,

    departmentID: null,
    designation: "",

    sexID: null,                            // here here here here here
    heightCM: 0,
    weightKG: 0,
    gsisID: "",
    pagibigID: "",
    philhealthID: "",
    sssNo: "",
    tin: "",
    agencyEmployeeNo: employeeID.value,
    residentialAddress: "",
    residentialZip: "",
    permanentAddress: "",
    permanentZip: "",
    isSameAddress: false,
    telephoneNo: "",
    mobileNo: "",
    email: personEmail.value,

    // 🔹 Spouse
    spouseSurname: "",
    spouseFirstName: "",
    spouseMiddleName: "",
    spouseNameExtension: "",
    spouseOccupation: "",
    spouseEmployer: "",
    spouseBusinessAddress: "",
    spouseTelephone: "",

    // 🔹 Father
    fatherSurname: "",
    fatherFirstName: "",
    fatherMiddleName: "",
    fatherNameExtension: "",

    // 🔹 Mother
    motherSurname: "",
    motherFirstName: "",
    motherMiddleName: "",
    motherNameExtension: "",


    // EDUCATION RECS

    elementaryName: "",
    elementaryType: "",
    elementaryStart: "",
    elementaryEnd: "",
    elementaryGraduated: "",

    secondaryName: "",
    secondaryType: "",
    secondaryStart: "",
    secondaryEnd: "",
    secondaryGraduated: "",

    vocationalName: "",
    vocationalType: "",
    vocationalStart: "",
    vocationalEnd: "",
    vocationalGraduated: "",

    collegeName: "",
    collegeType: "",
    collegeStart: "",
    collegeEnd: "",
    collegeGraduated: "",

    graduateName: "",
    graduateType: "",
    graduateStart: "",
    graduateEnd: "",
    graduateGraduated: "",




    // 🔹 new properties
    children: [],
    //educationRecords: [] = structuredClone(defaultEducation),
    voluntaryWorks: [],
    trainings: [],
    skillsHobbies: [],
    distinctions: [],
    memberships: [],
    civilServiceEligibilities: [],
    workExperiences: []
  });

  const genders = ref([]);
  const civilStatuses = ref([]);

  const departments = ref([]);

  const c2 = ref([]);
  const c3 = ref([]);
  const c4 = ref({ other: [], references: [] });

  // validations
  const validateEducation = () => {
    const errors = [];

    user.value.educationRecords.forEach((edu, idx) => {
      // ✅ Only validate if schoolName has a value
      if (edu.schoolName && edu.schoolName.trim() !== "") {
        if (!edu.attendanceFrom) {
          errors.push(`${edu.degree}: Year From is required`);
        }
        //if (!edu.attendanceTo) {
        //  errors.push(`${edu.degree}: Year To is required`);
        //}
        if (edu.attendanceTo) {
          if (edu.attendanceFrom && edu.attendanceTo && edu.attendanceFrom > edu.attendanceTo) {
            errors.push(`${edu.degree}: Year From cannot be after Year To`);
          }
          if (!edu.yearGraduated) {
            errors.push(`${edu.degree}: Year Graduated is required`);
          }
        }

        if (edu.yearGraduated) {
          if (edu.yearGraduated && edu.attendanceTo && edu.yearGraduated < edu.attendanceTo) {
            errors.push(`${edu.degree}: Year From cannot be after Year To`);
          }
        }

      }
    });

    return errors;
  };




  // ------------------ Load Data ------------------
  // Load dropdown options
  async function loadLookups() {
    try {
      isLoading.value = true; // show hourglass
      const g = await api.get("/employee/gender");
      genders.value = g.data;

      const cs = await api.get("/employee/civilstatus");
      civilStatuses.value = cs.data;

      const dpt = await api.get("/employee/departments");
      departments.value = dpt.data;

    } catch (err) {
      console.error("AxiosError:", err);

      if (err.response) {
        console.error("🔴 Backend responded with error:");
        console.error("Status:", err.response.status);
        console.error("Data:", err.response.data);
      } else if (err.request) {
        console.error("🔴 No response received from server");
        console.error("Request:", err.request);
      } else {
        console.error("🔴 Axios setup error:", err.message);
      }

      // Log out the user on error
      if (typeof auth.logout === "function") {
        auth.logout(); // this should clear user data and redirect to login
      } else {
        showDialog.value = true;
        dialogTitle.value = "Error";
        dialogMessage.value = "Failed to load dashboard stats. Please log in again.";
      }
    } finally {
      isLoading.value = false;  // hide hourglass
    }
  }


  async function loadData() {
    try {
      isLoading.value = true;
      const res = await api.get(`employee/user/${personId.value}`);
      const data = res.data;

      // helper to normalize dates for <input type="date">
      const formatDate = (dateStr) => {
        if (!dateStr) return "";
        return dateStr.substring(0, 10); // "yyyy-MM-dd"
      };

      // Personal Info
      user.value = {
        ...user.value, // keep defaults
        personID: data.personID,
        csidNo: data.csidNo ?? "",
        surname: data.surname ?? "",
        firstName: data.firstName ?? "",
        middleName: data.middleName ?? "",
        nameExtension: data.nameExtension ?? "",
        dateOfBirth: formatDate(data.dateOfBirth),
        placeOfBirth: data.placeOfBirth ?? "",
        bloodType: data.bloodType ?? "",
        citizenship: data.citizenship ?? "",
        civilStatusID: data.civilStatusID ?? null,
        sexID: data.sexID ?? null,

        departmentID: data.departmentID ?? null,
        designation: data.designation ?? null,

        heightCM: data.heightCM ?? null,
        weightKG: data.weightKG ?? null,
        gsisID: data.gsisID ?? "",
        pagibigID: data.pagibigID ?? "",
        philhealthID: data.philhealthID ?? "",
        sssNo: data.sssNo ?? "",
        tin: data.tin ?? "",
        agencyEmployeeNo: data.agencyEmployeeNo ?? "",
        residentialAddress: data.residentialAddress ?? "",
        residentialZip: data.residentialZip ?? "",
        permanentAddress: data.permanentAddress ?? "",
        permanentZip: data.permanentZip ?? "",
        telephoneNo: data.telephoneNo ?? "",
        mobileNo: data.mobileNo ?? "",
        email: data.email ?? "",
        // 🔹 Spouse
        spouseSurname: data.spouseSurname ?? "",
        spouseFirstName: data.spouseFirstName ?? "",
        spouseMiddleName: data.spouseMiddleName ?? "",
        spouseNameExtension: data.spouseNameExtension ?? "",
        spouseOccupation: data.spouseOccupation ?? "",
        spouseEmployer: data.spouseEmployer ?? "",
        spouseBusinessAddress: data.spouseBusinessAddress ?? "",
        spouseTelephone: data.spouseTelephone ?? "",

        // 🔹 Father
        fatherSurname: data.fatherSurname ?? "",
        fatherFirstName: data.fatherFirstName ?? "",
        fatherMiddleName: data.fatherMiddleName ?? "",
        fatherNameExtension: data.fatherNameExtension ?? "",

        // 🔹 Mother
        motherSurname: data.motherSurname ?? "",
        motherFirstName: data.motherFirstName ?? "",
        motherMiddleName: data.motherMiddleName ?? "",
        motherNameExtension: data.motherNameExtension ?? "",

        // 🔹 normalize children + education
        children: (data.children ?? []).map(c => ({
          ...c,
          dateOfBirth: formatDate(c.dateOfBirth)
        })),

        // educationRecords: (data.educationRecords ?? []).map(e => ({
        //   ...e,
        //   attendanceFrom: e.attendanceFrom ? Number(e.attendanceFrom) : null,
        //   attendanceTo: e.attendanceTo ? Number(e.attendanceTo) : null,
        //   yearGraduated: e.yearGraduated ? Number(e.yearGraduated) : null
        // })),


        //      EDUCATION RECORDS       //
        // ELEMENTARY EDUCATION
        elementaryName: data.elementaryName ?? "",
        elementaryType: data.elementaryType ?? "",
        elementaryStart: data.elementaryStart ?? "",
        elementaryEnd: data.elementaryEnd ?? "",
        elementaryGraduated: data.elementaryGraduated ?? "",

        // SECONDARY EDUCATION
        secondaryName: data.secondaryName ?? "",
        secondaryType: data.secondaryType ?? "",
        secondaryStart: data.secondaryStart ?? "",
        secondaryEnd: data.secondaryEnd ?? "",
        secondaryGraduated: data.secondaryGraduated ?? "",

        // VOCATIONAL / TRADE COURSE
        vocationalName: data.vocationalName ?? "",
        vocationalType: data.vocationalType ?? "",
        vocationalStart: data.vocationalStart ?? "",
        vocationalEnd: data.vocationalEnd ?? "",
        vocationalGraduated: data.vocationalGraduated ?? "",

        // COLLEGE
        collegeName: data.collegeName ?? "",
        collegeType: data.collegeType ?? "",
        collegeStart: data.collegeStart ?? "",
        collegeEnd: data.collegeEnd ?? "",
        collegeGraduated: data.collegeGraduated ?? "",

        // GRADUATE STUDIES
        graduateName: data.graduateName ?? "",
        graduateType: data.graduateType ?? "",
        graduateStart: data.graduateStart ?? "",
        graduateEnd: data.graduateEnd ?? "",
        graduateGraduated: data.graduateGraduated ?? "",


        civilServiceEligibilities: (data.civilServiceEligibilities ?? []).map(c => ({
          ...c,
          dateOfExamination: formatDate(c.dateOfExamination),
          dateReleased: formatDate(c.dateReleased)
        })),

        workExperiences: (data.workExperiences ?? []).map(w => ({
          ...w,
          dateFrom: formatDate(w.dateFrom),
          dateTo: formatDate(w.dateTo),
          salary: w.salary ?? null,
          isPresent: w.isPresent ?? false
        })),

        // Voluntary Work
        voluntaryWorks: (data.voluntaryWorks ?? []).map(v => ({
          ...v,
          dateFrom: formatDate(v.dateFrom),
          dateTo: formatDate(v.dateTo),
          numberOfHours: v.numberOfHours ?? null,
        })),

        // Trainings
        trainings: (data.trainings ?? []).map(t => ({
          ...t,
          dateFrom: formatDate(t.dateFrom),
          dateTo: formatDate(t.dateTo),
          numberOfHours: t.numberOfHours ?? null,
        })),

        // Skills & Hobbies
        skillsHobbies: (data.skillsHobbies ?? []).map(s => ({
          ...s,
          skillOrHobby: s.skillOrHobby ?? ""
        })),

        // Distinctions
        distinctions: (data.distinctions ?? []).map(d => ({
          ...d,
          distinction: d.distinction ?? ""
        })),

        // Memberships
        memberships: (data.memberships ?? []).map(m => ({
          ...m,
          organizationName: m.organizationName ?? ""
        }))
      };

      // ✅ Ensure education has default entries if null or empty
      if (!user.value.educationRecords || user.value.educationRecords.length === 0) {
        user.value.educationRecords = structuredClone(defaultEducation);
      }

      console.log("Loaded pds:", user.value);

    } catch (err) {
      console.error("Failed to load data:", err);
    } finally {
      isLoading.value = false;  // hide hourglass
    }
  }


  onMounted(() => {
    loadLookups();
    loadData();
  });


  // ------------------ Save Methods ------------------
  async function save() {
    dialogTitle.value = "Saving";
    isLoading.value = true;
    // ✅ validate education before save
    //const eduErrors = validateEducation();
    //if (eduErrors.length > 0) {
    //  dialogMessage.value = eduErrors.join("\n");
    //  showDialog.value = true;
    //  return; // stop save
    //}

    try {
      // Always enforce personID + email from token
      user.value.personID = personId.value;
      user.value.email = personEmail.value;

      // ✅ Normalize education numeric fields
      user.value.educationRecords = (user.value.educationRecords ?? []).map(e => ({
        ...e,
        attendanceFrom: e.attendanceFrom === "" ? null : e.attendanceFrom,
        attendanceTo: e.attendanceTo === "" ? null : e.attendanceTo,
        yearGraduated: e.yearGraduated === "" ? null : e.yearGraduated
      }));

      await api.put(`/employee/user/${user.value.personID}`, user.value);
      dialogTitle.value = "Success";
      dialogMessage.value = "User information saved successfully!";
      showDialog.value = true;
    } catch (err) {
      console.error("Failed to save user information:", err);
      dialogTitle.value = "Error";
      if (err.response?.data?.errors) {
        dialogMessage.value = Object.values(err.response.data.errors).flat().join("\n");
      } else {
        dialogMessage.value = "Failed to save user information.";
      }
      showDialog.value = true;
    } finally {
      isLoading.value = false;  // hide hourglass
    }
  }

  // ✅ Delete voluntary work
  function deleteVoluntaryWork(index) {
    user.value.voluntaryWorks.splice(index, 1);
  }
  // ✅ Delete training
  function deleteTraining(index) {
    user.value.trainings.splice(index, 1);
  }
  // ✅ Delete skillsHobby
  function deleteSkillOrHobby(index) {
    user.value.skillsHobbies.splice(index, 1);
  }
  // ✅ Delete distinction
  function deleteDistinction(index) {
    user.value.distinctions.splice(index, 1);
  }
  // ✅ Delete membership
  function deleteMemberShip(index) {
    user.value.memberships.splice(index, 1);
  }
  // ✅ Delete civil service eligibility
  function deleteCivilServiceEligibility(index) {
    user.value.civilServiceEligibilities.splice(index, 1);
  }
  // ✅ Delete work experience
  function deleteWorkExperience(index) {
    user.value.workExperiences.splice(index, 1);
  }
</script>


<style scoped>

  .delete-btn {
    background-color: #e74c3c;
    color: #fff;
    border: none;
    padding: 6px 12px;
    border-radius: 4px;
    cursor: pointer;
    font-size: 13px;
  }

    .delete-btn:hover {
      background-color: #c0392b;
    }

  /* Layout */
  .pds.layout {
    display: flex;
    /* min-height: 100vh; */
    /* height: 92vh; */
    /* width: 76vw; */
    /* width: 100%;
    height: 100%; */
    flex-direction: column;
  }

  .pds .content {
    flex: 1;
    padding: 15px;
    margin-top: 60px;
    /* margin-top: var(--header-height); */
    /* overflow-y: auto; */
    display: flex;
    flex-direction: column;
    /* background-color: black; */
    /* position: fixed; */
    /* bottom: 0; */
    /* right: 0; */
    /* height: 100%; */
    /* width: 100%; */
    width: calc(100% - var(--left-menu-width));
    height: calc(100% - var(--header-height));
  }

    /* Inputs */
    .pds .content input,
    .pds .content select,
    .pds .content textarea {
      width: 100%;
      padding: 6px 8px;
      border: 1px solid #ccc;
      border-radius: 4px;
      box-sizing: border-box;
      margin-bottom: 0.5rem;
    }

  .pds .form-wrapper { /* MAIN FORM HOLDER OF TABLE AND TITLES */
    display: grid;
    flex-direction: column;
    /* align-items: center; center each control */
    width: 100%;
    padding-left: 20px;
    /* margin: 15px; */
    /* max-width: 65vw; fixed form width */
    /* overflow-x: hidden; */
  }
    /* Inputs and selects */
    .pds .form-wrapper input,
    .pds .form-wrapper select,
    .pds .form-wrapper textarea {
      width: 100%; /* take wrapper width */
      max-width: 400px; /* keep controls uniform */
      /* margin-bottom: 12px; */
    }

  /* .pds .form-wrapper .personal-info-wrapper{
    display: grid;
    grid-template-columns: repeat(4, 1fr); 
    grid-template-rows: repeat(35, auto);  
    background: lightgrey;
  } */

  .sub-title {
    display: flex;
    /* background-color: lightgrey; */
    width: auto;
    color: #0c2884;
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    font-weight: 700;
    font-size: 1rem;
    border-bottom: 1px solid #f2f2f2;
    padding-top: 20px;
    padding-bottom: 20px;
    /* margin: 15px 0 20px -2.5vw; */
    /* margin-bottom: 10px; */
  }

  /* Buttons */
  .pds .btn {
    display: block;
    /* margin: 10px auto; */
    /* padding: 8px 16px; */
    border-radius: 50px;
    cursor: pointer;
    background: #2563eb;
    color: #fff;
    border: none;
    font-weight: 700;
    transition: background 0.2s ease;
    width: 50%;
    height: 43px;
  }
    .pds .btn:hover {
      background: #1d4ed8;
    }

  .pds .btn-white {
    display: block;
    /* margin: 10px auto; */
    /* padding: 8px 16px; */
    border-radius: 50px;
    cursor: pointer;
    background: white;
    color: #1d4ed8;
    /* border: none; */
    font-weight: 700;
    transition: background 0.2s ease;
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    font-size: 0.9rem;
    border: 1px solid #1d4ed8;
    width: 50%;
    height: 43px;
  }

    .pds .btn-white:hover {
      background: #fafafa;
    }

  /* Tabs */
  .pds .tab-container {
    display: flex;
    justify-content: flex-start;
    gap: 8px;
    /* gap: 125px; */
    /* margin-bottom: 0; remove space */
    margin-bottom: 10px;
    /* border-radius: 100%; */
    border-bottom: none; /* prevent double border */
    /* white-space:nowrap; prevent wrapping */
    border-radius:999px;
    background: white;
    box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1); /* subtle shadow */
  }

  .pds .tab-btn {
    flex: 1;
    min-width: 120px;
    padding: 10px 0;
    text-align: center;
    position: relative;
    background: none;
    /* border: 1px solid #ccc; */
    /* border-bottom: none; merge with content */
    cursor: pointer;
    /* border-radius: 6px 6px 0 0; rounded only on top */
    border: none;
    outline: none;
    background: none;
    box-shadow: none;
    color: #c0c0c0;
    font-weight: bold;
  }

    .pds .tab-btn.active {
      /* background: #fff; */
      border: 1px solid #ccc;
      border-bottom: none; /* 🔥 removes line under active tab */
      /* border-top: 3px solid #2563eb; */
      font-weight: bold;
      color: black;
      position: relative;
      outline: none;
      box-shadow: none;
      border: none;
      /* z-index: 2; ensures tab is above content */
    }

    .pds .tab-btn:focus{
      outline: none;
    }

  .pds .tab-btn:not(:last-child)::after {
    content: "";
    position: absolute;
    right: 0;
    top: 25%;
    height: 50%;
    width: 1px;
    background: #ccc; /* divider color */
  }

  /* Tab Wrapper */
  .pds .tab-wrapper {
    flex: 1;
    display: flex;
    flex-direction: column;
    /* height: calc(100vh - 160px); adjust header + tabs height */
    height: 78vh;
    box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1);
    border-radius: 12px;
  }

  /* Tab Content */
  .pds .tab-content {
    flex: 1 1 auto;
    min-height: 0;
    overflow-y: auto;
    overflow-x: hidden;
    /* border: 1px solid #ccc; */
    border-top: none;
    padding-bottom: 20px;
    background: #fff;
    border-radius: 0 6px 6px 6px;
    display: flex;
    justify-content: center; /* center horizontally */
    align-items: flex-start; /* align to top */
    border-radius: 12px;
  }
    .pds .tab-content input,
    .pds .tab-content select,
    .pds .tab-content textarea {
      width: 15vw; /* pick your desired fixed size */
      max-width: 100%; /* don't overflow parent */
      margin-bottom: 12px;
    }
    .pds .tab-content form {
      width: 100%;
    }

  /* Wrapper to center table */
  .pds-table-wrapper {
    /* display: flex; */
    justify-content: center; /* center horizontally */
    /* background: lightblue; */
    /* display: fixed; */
    /* margin: 20px 0; */
    /* width: 100%; */
    
    width: auto;
  }

  /* Table styling */
  .pds-table {
    border-collapse: collapse;
    /* width: 100%; */
    width: auto;
    table-layout: auto;
    /* max-width: 60vw; control max width */
    /* background: #fff; */
    /* background:lightgrey; */
    border: none;
  }

    .pds-table th,
    .pds-table td {
      border: 1px solid #ddd;
      padding: 0 8px 0 8px;
      text-align: left;
      border: none;
    }

    /* Inputs inside table */
    .pds-table input {
      width: 100%;
      box-sizing: border-box;
      padding: 6px 8px;
    }


  .note{
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    color: #ff5656;
    font-size: 0.55rem;
  }

  /* Responsive */
  @media (max-width: 768px) {
    .pds.layout {
      flex-direction: column;
    }

    .pds .tab-content {
      max-width: 100%;
      min-height: auto;
    }
  }

  /* @media only screen and (max-width: 800px) {
    table, thead, tbody, th, td, tr {
      width: 100%;
      border-collapse: collapse;
    }  
  } */

</style>
