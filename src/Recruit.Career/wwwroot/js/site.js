const app = Vue.createApp({
    data() {
        return {
            educations: [],
            experiences: [],
            selectedEducation: {},
            selectedExperience: {},
            showEducationForm: false,
            showExperienceForm: false,
            selectedIndex: null
        }
    },
    methods: {
        addEducation() {
            if (this.canAddEducation) {
                this.educations.push({
                    school: this.selectedEducation.school,
                    degree: this.selectedEducation.degree,
                    startDate: this.selectedEducation.startDate,
                    endDate: this.selectedEducation.endDate
                });
            }
        },
        addExperience() {
            if (this.canAddExperience) {
                this.experiences.push({
                    title: this.selectedExperience.title,
                    company: this.selectedExperience.company,
                    summary: this.selectedExperience.summary,
                    startDate: this.selectedExperience.startDate,
                    endDate: this.selectedExperience.endDate,
                    currentlyWorkingHere: this.selectedExperience.currentlyWorkingHere
                });
            }
        },
        updateEducation() {
            var item = this.educations[this.selectedIndex];
            if (item) {
                item.school = this.selectedEducation.school;
                item.degree = this.selectedEducation.degree;
                item.startDate = this.selectedEducation.startDate;
                item.endDate = this.selectedEducation.endDate;
            }
        },
        updateExperience() {
            var item = this.experiences[this.selectedIndex];
            if (item) {
                item.title = this.selectedExperience.title;
                item.company = this.selectedExperience.company;
                item.summary = this.selectedExperience.summary;
                item.startDate = this.selectedExperience.startDate;
                item.endDate = this.selectedExperience.endDate;
                item.currentlyWorkingHere = this.selectedExperience.currentlyWorkingHere;
            }
        },
        removeEducation(education) {
            this.educations.splice(this.educations.indexOf(education), 1);
        },
        removeExperience(experience) {
            this.experiences.splice(this.experiences.indexOf(experience), 1);
        },
        showCreateEducationForm() {
            this.selectedEducation = {};
            this.selectedIndex = null;
            this.showEducationForm = true;
        },
        showCreateExperienceForm() {
            this.selectedExperience = {};
            this.selectedIndex = null;
            this.showExperienceForm = true;
        },
        showEditEducationForm(index) {
            this.selectedEducation = JSON.parse(JSON.stringify(this.educations[index]));
            this.selectedIndex = index;
            this.showEducationForm = true;
        },
        showEditExperienceForm(index) {
            this.selectedExperience = JSON.parse(JSON.stringify(this.experiences[index]));
            this.selectedIndex = index;
            this.showExperienceForm = true;
        },
        handleEducationSubmit() {
            var isValidForm = this.selectedEducation.school?.trim();
            if (isValidForm) {
                if (this.isEdit) {
                    this.updateEducation();
                }
                else {
                    this.addEducation();
                }

                this.showEducationForm = false;
                this.selectedIndex = null;
                this.selectedEducation = {};
            }
        },
        handleExperienceSubmit() {
            var isValidForm = this.selectedExperience.title?.trim();
            if (isValidForm) {
                if (this.isEdit) {
                    this.updateExperience();
                }
                else {
                    this.addExperience();
                }

                this.showExperienceForm = false;
                this.selectedIndex = null;
                this.selectedExperience = {};
            }
        }
    },
    computed: {
        isEdit() {
            return this.selectedIndex !== null;
        },
        canAddEducation() {
            return this.educations.length < 5;
        },
        canAddExperience() {
            return this.experiences.length < 5;
        }
    }
});

app.mount("#app");