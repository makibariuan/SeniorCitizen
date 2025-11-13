<template>
  <div class="layout pds">
    <LeftMenu />
    <Header title="Details" />
    <div class="content">
      <div class="w-[90%] mx-auto">
        <!-- <h1 class="text-2xl font-bold mb-4">Personal Data Sheet</h1> -->
        <!-- Folder-style Tabs -->
        <!-- <div class="tab-container">
          <button v-for="tab in tabs"
                  :key="tab"
                  @click="activeTab = tab"
                  class="tab-btn"
                  :class="{ active: activeTab === tab }">
            {{ tab }}
          </button>
        </div> -->
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
                    <!-- <input v-model="user.csidNo" placeholder="CSID No" class="input" /> -->
                    <table class="pds-table">
                      <tbody>
                        <tr>
                          <td>
                            <p class="auth-input-label-bold">Surname*</p>
                            <input v-model="user.surname" placeholder="Surname" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label-bold">First Name*</p>
                            <input v-model="user.firstName" placeholder="First Name" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label-bold">Middle Name*</p>
                            <input v-model="user.middleName" placeholder="Middle Name" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label-bold">Sex</p>
                            <select v-model="user.sexID" class="auth-input">
                              <option disabled value="">-- Select Gender --</option>
                              <option v-for="g in genders" :key="g.genderID" :value="g.genderID">
                                {{ g.genderName }}
                              </option>
                            </select>
                          </td>
                          <!--<td>
                              <p class="auth-input-label-bold">Name Extension*</p>
                              <input v-model="user.nameExtension" placeholder="Name Extension (e.g., Jr., III)"   class="auth-input" />
                          </td>-->
                        </tr>
                        <tr>
                          <td>
                            <p class="auth-input-label-bold">Date of Birth*</p>
                            <input v-model="user.dateOfBirth" type="date" class="auth-input" />
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
                            <p class="auth-input-label-bold">Nationality*</p>
                            <input v-model="user.citizenship" placeholder="Citizenship" class="auth-input" />
                          </td>
                        </tr>
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
                          <td>
                            <p class="auth-input-label-bold">Issuing Authority</p>
                            <input placeholder="" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label-bold">Department</p>
                            <input placeholder="" class="auth-input" />
                          </td>
                          <td>
                            <p class="auth-input-label-bold">Employee ID No.</p>
                            <input placeholder="" class="auth-input" />
                          </td>
                        </tr>

                        <!-- ADDRESS -->
                        <tr>
                          <td colspan="3">
                            <p class="auth-input-label-bold">Residential Address*</p>
                            <input v-model="user.residentialAddress" placeholder="Residential Address" class="auth-input" />
                          </td>
                          <!--<td>
                              <p class="auth-input-label-bold">ZIP Code*</p>
                              <input v-model="user.residentialZip" placeholder="Residential Zip" class="auth-input" />
                          </td>-->
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
                            <input v-model="user.permanentAddress" placeholder="Permanent Address" class="auth-input" />
                          </td>
                          <!--<td>
                              <p class="auth-input-label-bold">ZIP Code*</p>
                              <input v-model="user.permanentZip" placeholder="Permanent Zip" class="auth-input" />
                          </td>-->
                        </tr>
                      </tbody>
                    </table>


                    <!-- FAMILY BACKGROUND -->
                    <h2 class="sub-title">Family Background</h2>
                    <div class="pds-table-wrapper">
                      <table class="pds-table">
                        <tbody>
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
                            <!--<td>
                                <p class="auth-input-label-bold">Father's Name Extension*</p>
                                <input v-model="user.fatherNameExtension" placeholder="Father Name Extension" class="auth-input" />
                            </td>-->
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
                            <!--<td>
                                <p class="auth-input-label-bold">Mother's Name Extension*</p>
                                <input v-model="user.motherNameExtension" placeholder="Mother Name Extension" class="auth-input" />
                            </td>-->
                          </tr>

                          <tr>
                            <td>
                              <!--<button @click="" class="btn-white" style="width: 100%">Go Back</button>-->
                            </td>
                            <td style="visibility: hidden">
                              <p class="auth-input-label-bold"></p>
                              <input class="input" disabled />
                            </td>
                            <td>
                              <button @click="save" class="btn-white" style="width: 100%">Save</button>
                            </td>
                            <td>
                              <!--<button @click="activeTab = 'Personal Data Sheet 2'" class="btn" style="width: 100%">Next</button>-->
                            </td>
                          </tr>

                        </tbody>
                      </table>
                    </div>
                  </div>
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
