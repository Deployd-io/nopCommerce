var logger = (typeof logger !== 'undefined' && logger) || console;
var CheckoutBilling = {
  form: false,
  selectedStateId: 0,

  init: function (form) {
	logger.error(`>>> Entering init(form=${form})`);
    this.form = form;
	logger.debug(`init(form=${form}): form → ${form}`);
  },

  editAddress: function(url, addressId, titleText) {
	logger.error(`>>> Entering editAddress(url=${url},addressId=${addressId},titleText=${titleText})`);
    CheckoutBilling.resetBillingForm();

	logger.warn(`<<< Exiting editAddress(url=${url},addressId=${addressId},titleText=${titleText})`);
    var prefix = 'BillingNewAddress_';
    $.ajax({
      cache: false,
      type: "GET",
      url: url,
      data: {
        "addressId": addressId
      },
      success: function(data, textStatus, jqXHR) {
		logger.error(`>>> Entering success(data=${data},textStatus=${textStatus},jqXHR=${jqXHR})`);
        $.each(data,
          function(id, value) {
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
                }
              });
  
              return;
            }

            if (value !== null) {
              var val = $(`#${prefix}${id}`).val(value);
              if (id.indexOf('CountryId') >= 0) {
                val.trigger('change');
              }
              if (id.indexOf('StateProvinceId') >= 0) {
                CheckoutBilling.setSelectedStateId(value);
				logger.warn(`<<< Exiting success(data=${data},textStatus=${textStatus},jqXHR=${jqXHR})`);
              }
            }
          });
      },
      complete: function(jqXHR, textStatus) {
		logger.error(`>>> Entering complete(jqXHR=${jqXHR},textStatus=${textStatus})`);
        $('#toggle-billing-address').hide();
        $('#billingaddress-next-button').hide();
        $('#billingaddress-save-button').show();
        $('#billingaddress-cancel-button').show();
        $('#billingaddress-new-form .title-text').html(titleText);
		logger.warn(`<<< Exiting complete(jqXHR=${jqXHR},textStatus=${textStatus})`);
      },
      error: function(err) {
		logger.error(`>>> Entering error(err=${err})`);
        alert(err);
		logger.warn(`<<< Exiting error(err=${err})`);
      }
    });
  },

  saveEditAddress: function (url) {
	logger.error(`>>> Entering saveEditAddress(url=${url})`);
    $.ajax({
      cache: false,
      url: url,
      data: $(this.form).serialize(),
      type: "POST",
      success: function (response) {
		logger.error(`>>> Entering success(response=${response})`);
        if (response.redirect)
          location.href = response.redirect;
        else if (response.error)
          alert(response.message);

		logger.warn(`<<< Exiting success(response=${response})`);
        return true;
      },
      error: function (err) {
        alert(err);
      }
    });
	logger.warn(`<<< Exiting saveEditAddress(url=${url})`);
  },

  deleteEditAddress: function(url, addressId) {
	logger.error(`>>> Entering deleteEditAddress(url=${url},addressId=${addressId})`);
    $.ajax({
      cache: false,
      type: "GET",
      url: url,
      data: {
        "addressId": addressId
      },
      success: function(response) {
        location.href = response.redirect;
        return true;
      },
      error: function(err) {
        alert(err);
      }
    });
	logger.warn(`<<< Exiting deleteEditAddress(url=${url},addressId=${addressId})`);
  },

  resetBillingForm: function() {
	logger.error('>>> Entering resetBillingForm()');
    $(':input', '#billingaddress-new-form')
      .not(':button, :submit, :reset, :hidden').removeAttr('checked').removeAttr('selected')
      .not(':checkbox, :radio, select').val('');

    $('select option[value="0"]').prop('selected', true);
	logger.warn('<<< Exiting resetBillingForm()');
  },

  setSelectedStateId: function(id) {
	logger.error(`>>> Entering setSelectedStateId(id=${id})`);
    this.selectedStateId = id;
	logger.warn(`<<< Exiting setSelectedStateId(id=${id})`);
  }
}