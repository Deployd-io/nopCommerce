const logger = console;
/*
** nopCommerce one page checkout
*/


var Checkout = {
    loadWaiting: false,
    failureUrl: false,

    init: function (failureUrl) {
		logger.debug(`>>> Entering init(failureUrl=${failureUrl})`);
        this.loadWaiting = false;
        this.failureUrl = failureUrl;

		logger.debug(`init(failureUrl=${failureUrl}): this.loadWaiting → ${this.loadWaiting}`);
		logger.debug(`init(failureUrl=${failureUrl}): this.failureUrl → ${this.failureUrl}`);
        Accordion.disallowAccessToNextSections = true;
		logger.debug(`init(failureUrl=${failureUrl}): Accordion.disallowAccessToNextSections → ${Accordion.disallowAccessToNextSections}`);
		logger.debug(`<<< Exiting init(failureUrl=${failureUrl})`);
		logger.debug(`>>> Entering init(form=${form},getAddressUrl=${getAddressUrl},saveUrl=${saveUrl},disableBillingAddressCheckoutStep=${disableBillingAddressCheckoutStep},guest=${guest})`);
		logger.debug(`init(form=${form},getAddressUrl=${getAddressUrl},saveUrl=${saveUrl},disableBillingAddressCheckoutStep=${disableBillingAddressCheckoutStep},guest=${guest}): this.form → ${this.form}`);
		logger.debug(`<<< Exiting init(form=${form},getAddressUrl=${getAddressUrl},saveUrl=${saveUrl},disableBillingAddressCheckoutStep=${disableBillingAddressCheckoutStep},guest=${guest})`);
    },

    ajaxFailure: function () {
		logger.debug('>>> Entering ajaxFailure()');
        location.href = Checkout.failureUrl;
		logger.debug('<<< Exiting ajaxFailure()');
		logger.debug(`ajaxFailure(): location.href → ${location.href}`);
    },

    _disableEnableAll: function (element, isDisabled) {
        var descendants = element.find('*');
        $(descendants).each(function () {
            if (isDisabled) {
                $(this).prop("disabled", true);
            } else {
                $(this).prop("disabled", false);
            }
        });

        if (isDisabled) {
            element.prop("disabled", true);
        } else {
            $(this).prop("disabled", false);
        }
    },

    setLoadWaiting: function (step, keepDisabled) {
		logger.debug(`>>> Entering setLoadWaiting(step=${step},keepDisabled=${keepDisabled})`);
        var container;
        if (step) {
            if (this.loadWaiting) {
                this.setLoadWaiting(false);
            }
            container = $('#' + step + '-buttons-container');
            container.addClass('disabled');
            container.css('opacity', '.5');
            this._disableEnableAll(container, true);
            $('#' + step + '-please-wait').show();
        } else {
            if (this.loadWaiting) {
                container = $('#' + this.loadWaiting + '-buttons-container');
                var isDisabled = keepDisabled ? true : false;
				logger.debug(`setLoadWaiting(step=${step},keepDisabled=${keepDisabled}): container → ${container}`);
                if (!isDisabled) {
                    container.removeClass('disabled');
                    container.css('opacity', '1');
                }
                this._disableEnableAll(container, isDisabled);
                $('#' + this.loadWaiting + '-please-wait').hide();
            }
        }
        this.loadWaiting = step;
		logger.debug(`setLoadWaiting(step=${step},keepDisabled=${keepDisabled}): this.loadWaiting → ${this.loadWaiting}`);
		logger.debug(`<<< Exiting setLoadWaiting(step=${step},keepDisabled=${keepDisabled})`);
    },

    gotoSection: function (section) {
		logger.debug(`>>> Entering gotoSection(section=${section})`);
        section = $('#opc-' + section);
        section.addClass('allow');
		logger.debug(`gotoSection(section=${section}): section → ${section}`);
        Accordion.openSection(section);
		logger.debug(`<<< Exiting gotoSection(section=${section})`);
    },

    back: function () {
		logger.debug('>>> Entering back()');
        if (this.loadWaiting) return;
        Accordion.openPrevSection(true, true);
		logger.debug('<<< Exiting back()');
    },

    setStepResponse: function(response) {
	logger.debug(`>>> Entering setStepResponse(response=${response})`);
      if (response.update_section) {
        $('#checkout-' + response.update_section.name + '-load').html(response.update_section.html);
      }
      if (response.allow_sections) {
        response.allow_sections.each(function(e) {
          $('#opc-' + e).addClass('allow');
        });
      }

      //TODO move it to a new method
      if ($("#billing-address-select").length > 0) {
        Billing.newAddress(!$('#billing-address-select').val());
      } else {
        Billing.newAddress(true);
      }

      if ($("#shipping-address-select").length > 0) {
        Shipping.newAddress(response.selected_id == undefined ? $('#shipping-address-select').val() : response.selected_id, $('#billing-address-select').children("option:selected").val());
      }

      if (response.goto_section) {
        Checkout.gotoSection(response.goto_section);
        return true;
      }
      if (response.redirect) {
        location.href = response.redirect;
		logger.debug(`setStepResponse(response=${response}): location.href → ${location.href}`);
		logger.debug(`<<< Exiting setStepResponse(response=${response})`);
        return true;
      }
	logger.debug('>>> Entering function()');
	logger.debug('<<< Exiting function()');
      return false;
    }
};


var Billing = {
  form: false,
  getAddressUrl: '',
  saveUrl: '',
  disableBillingAddressCheckoutStep: false,
  guest: false,
  selectedStateId: 0,  

  init: function (form, getAddressUrl, saveUrl, disableBillingAddressCheckoutStep, guest) {
	logger.debug(`>>> Entering init(form=${form},getAddressUrl=${getAddressUrl},saveUrl=${saveUrl},disableBillingAddressCheckoutStep=${disableBillingAddressCheckoutStep},guest=${guest})`);
    this.form = form;    
    this.getAddressUrl = getAddressUrl;
    this.saveUrl = saveUrl;
    this.disableBillingAddressCheckoutStep = disableBillingAddressCheckoutStep;
    this.guest = guest;
	logger.debug(`<<< Exiting init(form=${form},getAddressUrl=${getAddressUrl},saveUrl=${saveUrl},disableBillingAddressCheckoutStep=${disableBillingAddressCheckoutStep},guest=${guest})`);
	logger.debug(`>>> Entering init(form=${form},saveUrl=${saveUrl})`);
	logger.debug(`init(form=${form},saveUrl=${saveUrl}): this.saveUrl → ${this.saveUrl}`);
  },

  newAddress: function (isNew) {
	logger.debug(`>>> Entering newAddress(isNew=${isNew})`);
    $('#save-billing-address-button').hide();

    if (isNew) {
      $('#billing-new-address-form').show();
      $('#edit-billing-address-button').hide();
      $('#delete-billing-address-button').hide();
    } else {      
      $('#billing-new-address-form').hide();
      $('#edit-billing-address-button').show();
      $('#delete-billing-address-button').show();
    }
    $(document).trigger({ type: "onepagecheckout_billing_address_new" });
    Billing.initializeCountrySelect();

    if (isNew)
	logger.debug(`<<< Exiting newAddress(isNew=${isNew})`);
      Billing.editAddress();
	logger.debug(`>>> Entering newAddress(id=${id},billingAddressId=${billingAddressId})`);
	logger.debug(`<<< Exiting newAddress(id=${id},billingAddressId=${billingAddressId})`);
  },

  setDefaultCountry: function (defaultCountry) {
	logger.debug(`>>> Entering setDefaultCountry(defaultCountry=${defaultCountry})`);
    $('#opc-billing select[data-trigger="country-select"] option[value="' + defaultCountry + '"]').prop('selected', true);
    $('#opc-billing select[data-trigger="country-select"] option:selected').change()
  },
  
  resetSelectedAddress: function() {
	logger.debug('>>> Entering resetSelectedAddress()');
    var selectElement = $('#billing-address-select');
    if (selectElement) {
      selectElement.val('');
    }
    $(document).trigger({ type: "onepagecheckout_billing_address_reset" });
	logger.debug('<<< Exiting resetSelectedAddress()');
	logger.debug(`resetSelectedAddress(): selectElement → ${selectElement}`);
  },

  save: function() {
	logger.debug('>>> Entering save()');
    if (Checkout.loadWaiting !== false) return;

	logger.debug('<<< Exiting save()');
    Checkout.setLoadWaiting('billing');

    $.ajax({
      cache: false,
      url: this.saveUrl,
      data: $(this.form).serialize(),
      type: "POST",
      success: this.nextStep,
      complete: this.resetLoadWaiting,
      error: Checkout.ajaxFailure
    });
  },

  resetLoadWaiting: function() {
	logger.debug('>>> Entering resetLoadWaiting()');
    Checkout.setLoadWaiting(false);
	logger.debug('<<< Exiting resetLoadWaiting()');
  },

  nextStep: function(response) {
	logger.debug(`>>> Entering nextStep(response=${response})`);
    //ensure that response.wrong_billing_address is set
    //if not set, "true" is the default value
    if (typeof response.wrong_billing_address === 'undefined') {
      response.wrong_billing_address = false;
    }
    if (Billing.disableBillingAddressCheckoutStep) {
	logger.debug(`nextStep(response=${response}): response.wrong_billing_address → ${response.wrong_billing_address}`);
      if (response.wrong_billing_address) {
        Accordion.showSection('#opc-billing');
      } else {
        Accordion.hideSection('#opc-billing');
      }
    }


    if (response.error) {
      if (typeof response.message === 'string') {
        alert(response.message);
      } else {
        alert(response.message.join("\n"));
      }

      return false;
    }

    Checkout.setStepResponse(response);
    Billing.initializeCountrySelect();
	logger.debug(`<<< Exiting nextStep(response=${response})`);
  },

  initializeCountrySelect: function() {
	logger.debug('>>> Entering initializeCountrySelect()');
    if ($('#opc-billing').has('select[data-trigger="country-select"]')) {
      $('#opc-billing select[data-trigger="country-select"]').countrySelect();
	logger.debug('<<< Exiting initializeCountrySelect()');
    }
  },

  editAddress: function() {
	logger.debug('>>> Entering editAddress()');
    Billing.resetBillingForm();

    var prefix = 'BillingNewAddress_';
	logger.debug('<<< Exiting editAddress()');
    var selectedItem = $('#billing-address-select').children("option:selected").val();

    $.ajax({
      cache: false,
      type: "GET",
      url: this.getAddressUrl,
      data: {
        addressId: selectedItem,
      },
      success: function (data, textStatus, jqXHR) {
		logger.debug(`>>> Entering success(data=${data},textStatus=${textStatus},jqXHR=${jqXHR})`);
        $.each(data, function (id, value) {
          if (value === null)
            return;

          if (id.indexOf("CustomAddressAttributes") >= 0 && Array.isArray(value)) {
            $.each(value, function (i, customAttribute) {
              if (customAttribute.DefaultValue) {
                $(`#${customAttribute.ControlId}`).val(
                  customAttribute.DefaultValue
                );
              } else {
                $.each(customAttribute.Values, function (j, attributeValue) {
                  if (attributeValue.IsPreSelected) {
                    $(`#${customAttribute.ControlId}`).val(attributeValue.Id);
                    $(
                      `#${customAttribute.ControlId}_${attributeValue.Id}`
                    ).prop("checked", attributeValue.Id);
                  }
                });
				logger.debug(`<<< Exiting each(value=${value})`);
              }
            });

            return;
          }

          var val = $(`#${prefix}${id}`).val(value);
          if (id.indexOf("CountryId") >= 0) {
            val.trigger("change");
          }
          if (id.indexOf("StateProvinceId") >= 0) {
            Billing.setSelectedStateId(value);
          }
        });
		logger.debug(`<<< Exiting success(data=${data},textStatus=${textStatus},jqXHR=${jqXHR})`);
		logger.debug(`>>> Entering success(response=${response})`);
		logger.debug(`<<< Exiting success(response=${response})`);
      },
      complete: function (jqXHR, textStatus) {
		logger.debug(`>>> Entering complete(jqXHR=${jqXHR},textStatus=${textStatus})`);
        $("#billing-new-address-form").show();
		logger.debug(`<<< Exiting complete(jqXHR=${jqXHR},textStatus=${textStatus})`);
        $("#edit-billing-address-button").hide();
        $("#delete-billing-address-button").hide();
        if (selectedItem != 0) {
          $("#save-billing-address-button").show();
        }
		logger.debug('>>> Entering complete()');
		logger.debug('<<< Exiting complete()');
      },
      error: Checkout.ajaxFailure,
    });
	logger.debug(`>>> Entering editAddress(url=${url})`);
	logger.debug(`<<< Exiting editAddress(url=${url})`);
  },

  saveEditAddress: function(url) {
	logger.debug(`>>> Entering saveEditAddress(url=${url})`);
    var selectedId;
    $.ajax({
      cache: false,
      url: url + '?opc=true',
      data: $(this.form).serialize(),
      type: "POST",
      success: function (response) {
        if (response.error) {
          alert(response.message);
          return false;
        } else {
          selectedId = response.selected_id;
          Checkout.setStepResponse(response);
          Billing.resetBillingForm();
        }        
      },
      complete: function() {
        var selectElement = $('#billing-address-select');
        if (selectElement && selectedId) {
          selectElement.val(selectedId);
        }
      },
      error: Checkout.ajaxFailure
    });
	logger.debug(`<<< Exiting saveEditAddress(url=${url})`);
	logger.debug(`saveEditAddress(url=${url}): selectedId → ${selectedId}`);
  },

  deleteAddress: function (url) {
	logger.debug(`>>> Entering deleteAddress(url=${url})`);
    var selectedAddress = $('#billing-address-select').children("option:selected").val();
    $.ajax({
      cache: false,
      type: "GET",
      url: url,
      data: {
        "addressId": selectedAddress,
        "opc": 'true'
      },
      success: function (response) {
        Checkout.setStepResponse(response);
      },
      error: Checkout.ajaxFailure
    });
	logger.debug(`<<< Exiting deleteAddress(url=${url})`);
  },

  setSelectedStateId: function (id) {
	logger.debug(`>>> Entering setSelectedStateId(id=${id})`);
    this.selectedStateId = id;
	logger.debug(`<<< Exiting setSelectedStateId(id=${id})`);
	logger.debug(`setSelectedStateId(id=${id}): this.selectedStateId → ${this.selectedStateId}`);
  },

  resetBillingForm: function() {
	logger.debug('>>> Entering resetBillingForm()');
    $(':input', '#billing-new-address-form')
      .not(':button, :submit, :reset, :hidden')
      .removeAttr('checked').removeAttr('selected')
    $(':input', '#billing-new-address-form')
      .not(':checkbox, :radio, select')
      .val('');

    $('.address-id', '#billing-new-address-form').val('0');
    $('select option[value="0"]', '#billing-new-address-form').prop('selected', true);
	logger.debug('<<< Exiting resetBillingForm()');
  }
};

var Shipping = {
    form: false,
    saveUrl: false,

    init: function (form, saveUrl) {
		logger.debug(`>>> Entering init(form=${form},saveUrl=${saveUrl})`);
        this.form = form;
        this.saveUrl = saveUrl;
		logger.debug(`init(form=${form},saveUrl=${saveUrl}): this.form → ${this.form}`);
		logger.debug(`<<< Exiting init(form=${form},saveUrl=${saveUrl})`);
		logger.debug(`>>> Entering init(form=${form},saveUrl=${saveUrl},localized_data=${localized_data})`);
		logger.debug(`init(form=${form},saveUrl=${saveUrl},localized_data=${localized_data}): this.localized_data → ${this.localized_data}`);
    },

  newAddress: function (id, billingAddressId) {
    isNew = !id;    
        if (isNew) {
          this.resetSelectedAddress();         
          $('#shipping-new-address-form').show();          
          $('#edit-shipping-address-button').hide();
          $('#delete-shipping-address-button').hide();
        } else {
          $('#shipping-new-address-form').hide();
          if (id == billingAddressId || (id != undefined && billingAddressId == undefined)) {
            $('#edit-shipping-address-button').hide();
            $("#save-shipping-address-button").hide();
            $('#delete-shipping-address-button').hide();            
          } else {
            $("#save-shipping-address-button").hide();
            $('#edit-shipping-address-button').show();
            $('#delete-shipping-address-button').show();
          }
        }
        $(document).trigger({ type: "onepagecheckout_shipping_address_new" });
        Shipping.initializeCountrySelect();
    },

    resetSelectedAddress: function () {
		logger.debug('>>> Entering resetSelectedAddress()');
        var selectElement = $('#shipping-address-select');
        if (selectElement) {
            selectElement.val('');
        }
        $(document).trigger({ type: "onepagecheckout_shipping_address_reset" });
		logger.debug('<<< Exiting resetSelectedAddress()');
  },

    editAddress: function (url) {
      Shipping.resetShippingForm();

      var prefix = 'ShippingNewAddress_';
      var selectedItem = $('#shipping-address-select').children("option:selected").val();
      $.ajax({
        cache: false,
        type: "GET",
        url: url,
        data: {
          addressId: selectedItem,
        },
        success: function (data, textStatus, jqXHR) {
          $.each(data, function (id, value) {
            if (value === null)
              return;

            if (id.indexOf("CustomAddressAttributes") >= 0 && Array.isArray(value)) {
              $.each(value, function (i, customAttribute) {
				logger.debug(`editAddress(): prefix → ${prefix}`);
				logger.debug(`editAddress(): selectedItem → ${selectedItem}`);
                if (customAttribute.DefaultValue) {
                  $(`#${customAttribute.ControlId}`).val(
                    customAttribute.DefaultValue
                  );
                } else {
				logger.debug(`editAddress(url=${url}): prefix → ${prefix}`);
				logger.debug(`editAddress(url=${url}): selectedItem → ${selectedItem}`);
                  $.each(customAttribute.Values, function (j, attributeValue) {
                    if (attributeValue.IsPreSelected) {
                      $(`#${customAttribute.ControlId}`).val(attributeValue.Id);
                      $(
                        `#${customAttribute.ControlId}_${attributeValue.Id}`
                      ).prop("checked", attributeValue.Id);
                    }
                  });
                }
              });

              return;
            }

            var val = $(`#${prefix}${id}`).val(value);
            if (id.indexOf("CountryId") >= 0) {
              val.trigger("change");
            }    
            if (id.indexOf("StateProvinceId") >= 0) {
              Billing.setSelectedStateId(value);
            }
          });
        },
        complete: function (jqXHR, textStatus) {
          $("#shipping-new-address-form").show();
          $("#edit-shipping-address-button").hide();
          $("#delete-shipping-address-button").hide();
          $("#save-shipping-address-button").show();
		logger.debug(`complete(): selectElement → ${selectElement}`);
        },
        error: Checkout.ajaxFailure,
      });
    },

    saveEditAddress: function (url) {
      var selectedId;
      $.ajax({
        cache: false,
        url: url + '?opc=true',
        data: $(this.form).serialize(),
        type: "POST",
        success: function (response) {
          if (response.error) {
            alert(response.message);
            return false;
          } else {
            selectedId = response.selected_id;
            Checkout.setStepResponse(response);
            Shipping.resetShippingForm();
			logger.debug(`<<< Exiting success(response=${response})`);
          }
        },
        complete: function () {
          var selectElement = $('#shipping-address-select');
          if (selectElement && selectedId) {
            selectElement.val(selectedId);
			logger.debug('<<< Exiting complete()');
          }
        },
        error: Checkout.ajaxFailure
      });
    },

    deleteAddress: function (url) {
      var selectedAddress = $('#shipping-address-select').children("option:selected").val();
      $.ajax({
        cache: false,
        type: "GET",
        url: url,
        data: {
          "addressId": selectedAddress,
          "opc": 'true'
        },
        success: function (response) {
          Checkout.setStepResponse(response);
        },
        error: Checkout.ajaxFailure
      });
    },

    save: function () {
		logger.debug('>>> Entering save()');
        if (Checkout.loadWaiting !== false) return;

		logger.debug('<<< Exiting save()');
        Checkout.setLoadWaiting('shipping');

        $.ajax({
            cache: false,
            url: this.saveUrl,
            data: $(this.form).serialize(),
            type: "POST",
            success: this.nextStep,
            complete: this.resetLoadWaiting,
            error: Checkout.ajaxFailure
        });
    },

    resetLoadWaiting: function () {
		logger.debug('>>> Entering resetLoadWaiting()');
        Checkout.setLoadWaiting(false);
		logger.debug('<<< Exiting resetLoadWaiting()');
    },

    nextStep: function (response) {
		logger.debug(`>>> Entering nextStep(response=${response})`);
        if (response.error) {
            if (typeof response.message === 'string') {
                alert(response.message);
            } else {
                alert(response.message.join("\n"));
            }

            return false;
        }

        Checkout.setStepResponse(response);
		logger.debug(`<<< Exiting nextStep(response=${response})`);
    },

    initializeCountrySelect: function () {
		logger.debug('>>> Entering initializeCountrySelect()');
        if ($('#opc-shipping').has('select[data-trigger="country-select"]')) {
            $('#opc-shipping select[data-trigger="country-select"]').countrySelect();
			logger.debug('<<< Exiting initializeCountrySelect()');
        }
  },

    resetShippingForm: function () {
	logger.debug('>>> Entering resetShippingForm()');
      $(':input', '#shipping-new-address-form')
        .not(':button, :submit, :reset, :hidden')
        .removeAttr('checked').removeAttr('selected')
      $(':input', '#shipping-new-address-form')
        .not(':checkbox, :radio, select')
        .val('');

      $('.address-id', '#shipping-new-address-form').val('0');
      $('select option[value="0"]', '#shipping-new-address-form').prop('selected', true);
	logger.debug('<<< Exiting resetShippingForm()');
    }
};



var ShippingMethod = {
    form: false,
    saveUrl: false,
    localized_data: false,

    init: function (form, saveUrl, localized_data) {
        this.form = form;
        this.saveUrl = saveUrl;
        this.localized_data = localized_data;
		logger.debug(`init(form=${form},saveUrl=${saveUrl},localized_data=${localized_data}): this.form → ${this.form}`);
		logger.debug(`<<< Exiting init(form=${form},saveUrl=${saveUrl},localized_data=${localized_data})`);
    },

    validate: function () {
        var methods = document.getElementsByName('shippingoption');
        if (methods.length === 0) {
            alert(this.localized_data.NotAvailableMethodsError);
            return false;
        }

        for (var i = 0; i < methods.length; i++) {
            if (methods[i].checked) {
                return true;
            }
        }
        alert(this.localized_data.SpecifyMethodError);
        return false;
    },

    save: function () {
        if (Checkout.loadWaiting !== false) return;

        if (this.validate()) {
			logger.debug('<<< Exiting save()');
            Checkout.setLoadWaiting('shipping-method');

            $.ajax({
                cache: false,
                url: this.saveUrl,
                data: $(this.form).serialize(),
                type: "POST",
                success: this.nextStep,
                complete: this.resetLoadWaiting,
                error: Checkout.ajaxFailure
            });
        }
    },

    resetLoadWaiting: function () {
        Checkout.setLoadWaiting(false);
    },

    nextStep: function (response) {
        if (response.error) {
            if (typeof response.message === 'string') {
                alert(response.message);
            } else {
                alert(response.message.join("\n"));
            }

            return false;
        }

        Checkout.setStepResponse(response);
    }
};



var PaymentMethod = {
    form: false,
    saveUrl: false,
    localized_data: false,

    init: function (form, saveUrl, localized_data) {
        this.form = form;
        this.saveUrl = saveUrl;
        this.localized_data = localized_data;
    },

    toggleUseRewardPoints: function (useRewardPointsInput) {
		logger.debug(`>>> Entering toggleUseRewardPoints(useRewardPointsInput=${useRewardPointsInput})`);
        if (useRewardPointsInput.checked) {
            $('#payment-method-block').hide();
        }
        else {
            $('#payment-method-block').show();
			logger.debug(`<<< Exiting toggleUseRewardPoints(useRewardPointsInput=${useRewardPointsInput})`);
        }
    },

    validate: function () {
        var methods = document.getElementsByName('paymentmethod');
        if (methods.length === 0) {
            alert(this.localized_data.NotAvailableMethodsError);
            return false;
        }

        for (var i = 0; i < methods.length; i++) {
            if (methods[i].checked) {
                return true;
            }
        }
        alert(this.localized_data.SpecifyMethodError);
        return false;
    },

    save: function () {
        if (Checkout.loadWaiting !== false) return;

        if (this.validate()) {
            Checkout.setLoadWaiting('payment-method');
            $.ajax({
                cache: false,
                url: this.saveUrl,
                data: $(this.form).serialize(),
                type: "POST",
                success: this.nextStep,
                complete: this.resetLoadWaiting,
                error: Checkout.ajaxFailure
            });
        }
    },

    resetLoadWaiting: function () {
        Checkout.setLoadWaiting(false);
    },

    nextStep: function (response) {
        if (response.error) {
            if (typeof response.message === 'string') {
                alert(response.message);
            } else {
                alert(response.message.join("\n"));
            }

            return false;
        }

        Checkout.setStepResponse(response);
    }
};



var PaymentInfo = {
    form: false,
    saveUrl: false,

    init: function (form, saveUrl) {
        this.form = form;
        this.saveUrl = saveUrl;
		logger.debug(`>>> Entering init(saveUrl=${saveUrl},successUrl=${successUrl},isCaptchaEnabled=${isCaptchaEnabled},isReCaptchaV3=${isReCaptchaV3},recaptchaPublicKey=${recaptchaPublicKey},div=${div})`);
		logger.debug(`init(saveUrl=${saveUrl},successUrl=${successUrl},isCaptchaEnabled=${isCaptchaEnabled},isReCaptchaV3=${isReCaptchaV3},recaptchaPublicKey=${recaptchaPublicKey},div=${div}): div → ${div}`);
		logger.debug(`init(form=${form},getAddressUrl=${getAddressUrl},saveUrl=${saveUrl},disableBillingAddressCheckoutStep=${disableBillingAddressCheckoutStep},guest=${guest}): saveUrl → ${saveUrl}`);
		logger.debug(`init(form=${form},saveUrl=${saveUrl}): saveUrl → ${saveUrl}`);
		logger.debug(`init(form=${form},saveUrl=${saveUrl},localized_data=${localized_data}): saveUrl → ${saveUrl}`);
		logger.debug(`init(saveUrl=${saveUrl},successUrl=${successUrl},isCaptchaEnabled=${isCaptchaEnabled},isReCaptchaV3=${isReCaptchaV3},recaptchaPublicKey=${recaptchaPublicKey},div=${div}): this.recaptchaPublicKey → ${this.recaptchaPublicKey}`);
    },

    save: function () {
        if (Checkout.loadWaiting !== false) return;

        Checkout.setLoadWaiting('payment-info');
        $.ajax({
            cache: false,
            url: this.saveUrl,
            data: $(this.form).serialize(),
            type: "POST",
            success: this.nextStep,
            complete: this.resetLoadWaiting,
            error: Checkout.ajaxFailure
        });
    },

    resetLoadWaiting: function () {
        Checkout.setLoadWaiting(false);
		logger.debug(`>>> Entering resetLoadWaiting(transport=${transport})`);
		logger.debug(`<<< Exiting resetLoadWaiting(transport=${transport})`);
    },

    nextStep: function (response) {
        if (response.error) {
            if (typeof response.message === 'string') {
                alert(response.message);
            } else {
                alert(response.message.join("\n"));
            }

            return false;
        }

        Checkout.setStepResponse(response);
		logger.debug(`nextStep(response=${response}): location.href → ${location.href}`);
		logger.debug(`nextStep(response=${response}): window.location → ${window.location}`);
    }
};



var ConfirmOrder = {
    form: false,    
    saveUrl: false,
    isSuccess: false,
    isCaptchaEnabled: false,
    isReCaptchaV3: false,
    recaptchaPublicKey: "",
    div: false,

  init: function (saveUrl, successUrl, isCaptchaEnabled, isReCaptchaV3, recaptchaPublicKey, div) {
        this.div = div;
        this.saveUrl = saveUrl;
		logger.debug(`init(saveUrl=${saveUrl},successUrl=${successUrl},isCaptchaEnabled=${isCaptchaEnabled},isReCaptchaV3=${isReCaptchaV3},recaptchaPublicKey=${recaptchaPublicKey},div=${div}): isReCaptchaV3 → ${isReCaptchaV3}`);
		logger.debug(`init(saveUrl=${saveUrl},successUrl=${successUrl},isCaptchaEnabled=${isCaptchaEnabled},isReCaptchaV3=${isReCaptchaV3},recaptchaPublicKey=${recaptchaPublicKey},div=${div}): this.div → ${this.div}`);
        this.successUrl = successUrl;
        this.isCaptchaEnabled = isCaptchaEnabled;
        this.isReCaptchaV3 = isReCaptchaV3;
        this.recaptchaPublicKey = recaptchaPublicKey;
		logger.debug(`<<< Exiting init(saveUrl=${saveUrl},successUrl=${successUrl},isCaptchaEnabled=${isCaptchaEnabled},isReCaptchaV3=${isReCaptchaV3},recaptchaPublicKey=${recaptchaPublicKey},div=${div})`);
    },

  save: async function () {
    if (Checkout.loadWaiting !== false) return;

      //terms of service
        var termOfServiceOk = true;
        if ($('#termsofservice').length > 0) {
            //terms of service element exists
            if (!$('#termsofservice').is(':checked')) {
                $("#terms-of-service-warning-box").dialog();
                termOfServiceOk = false;
            } else {
                termOfServiceOk = true;
            }
        }
        if (termOfServiceOk) {
            Checkout.setLoadWaiting('confirm-order');
            var postData = {};

            if (ConfirmOrder.isCaptchaEnabled) {
                var captchaTok = await ConfirmOrder.getCaptchaToken('OpcConfirmOrder');
                postData['g-recaptcha-response'] = captchaTok;
            }

            addAntiForgeryToken(postData);
            $.ajax({
                cache: false,
                url: this.saveUrl,
                data: postData,
                type: "POST",
                success: this.nextStep,
                complete: this.resetLoadWaiting,
                error: Checkout.ajaxFailure
            });
        } else {
            return false;
        }
    },

    getCaptchaToken: async function (action) {
        var recaptchaToken = ''
        if (ConfirmOrder.isReCaptchaV3) {
            grecaptcha.ready(() => {
                grecaptcha.execute(this.recaptchaPublicKey, { action: action }).then((token) => {
                    recaptchaToken = token;
                });
            });
            while (recaptchaToken == '') {
              await new Promise(t => setTimeout(t, 100));
            }
        } else {
          recaptchaToken = $(this.div).find('.captcha-box textarea[name="g-recaptcha-response"]').val();
        }

        return recaptchaToken;
    },

    resetLoadWaiting: function (transport) {
        Checkout.setLoadWaiting(false, ConfirmOrder.isSuccess);
    },

    nextStep: function (response) {
        if (response.error) {
            if (typeof response.message === 'string') {
                alert(response.message);
            } else {
                alert(response.message.join("\n"));
            }

            return false;
        }

        if (response.redirect) {
            ConfirmOrder.isSuccess = true;
			logger.debug(`nextStep(response=${response}): ConfirmOrder.isSuccess → ${ConfirmOrder.isSuccess}`);
            location.href = response.redirect;
            return;
        }
        if (response.success) {
            ConfirmOrder.isSuccess = true;
            window.location = ConfirmOrder.successUrl;
        }

        Checkout.setStepResponse(response);
    }
};
