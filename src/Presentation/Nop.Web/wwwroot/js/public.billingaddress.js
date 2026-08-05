const logger = console;
﻿var CheckoutBilling = {
  form: false,
  selectedStateId: 0,

  init: function (form) {
	logger.debug(`>>> Entering init(form=${form})`);
    this.form = form;
	logger.debug(`<<< Exiting init(form=${form})`);
	logger.debug(`init(form=${form}): form → ${form}`);
  },

  editAddress: function(url, addressId, titleText) {
	logger.debug(`>>> Entering editAddress(url=${url},addressId=${addressId},titleText=${titleText})`);
    CheckoutBilling.resetBillingForm();

	logger.debug(`<<< Exiting editAddress(url=${url},addressId=${addressId},titleText=${titleText})`);
    var prefix = 'BillingNewAddress_';
    $.ajax({
      cache: false,
      type: "GET",
      url: url,
      data: {
        "addressId": addressId
      },
      success: function(data, textStatus, jqXHR) {
		logger.debug(`>>> Entering success(data=${data},textStatus=${textStatus},jqXHR=${jqXHR})`);
        $.each(data,
          function(id, value) {
		logger.debug(`<<< Exiting success(data=${data},textStatus=${textStatus},jqXHR=${jqXHR})`);
            if (id.indexOf("CustomAddressAttributes") >= 0 && Array.isArray(value)) {
              $.each(value, function (i, customAttribute) {
				logger.debug(`>>> Entering each(value=${value})`);
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
			logger.debug(`function(id=${id},value=${value}): val → ${val}`);
              if (id.indexOf('CountryId') >= 0) {
                val.trigger('change');
              }
              if (id.indexOf('StateProvinceId') >= 0) {
                CheckoutBilling.setSelectedStateId(value);
              }
            }
          });
		logger.debug(`>>> Entering success(response=${response})`);
		logger.debug(`<<< Exiting success(response=${response})`);
		logger.debug(`success(response=${response}): location.href → ${location.href}`);
      },
      complete: function(jqXHR, textStatus) {
		logger.debug(`>>> Entering complete(jqXHR=${jqXHR},textStatus=${textStatus})`);
        $('#toggle-billing-address').hide();
        $('#billingaddress-next-button').hide();
        $('#billingaddress-save-button').show();
        $('#billingaddress-cancel-button').show();
        $('#billingaddress-new-form .title-text').html(titleText);
		logger.debug(`<<< Exiting complete(jqXHR=${jqXHR},textStatus=${textStatus})`);
      },
      error: function(err) {
		logger.debug(`>>> Entering error(err=${err})`);
        alert(err);
		logger.debug(`<<< Exiting error(err=${err})`);
      }
    });
  },

  saveEditAddress: function (url) {
	logger.debug(`>>> Entering saveEditAddress(url=${url})`);
    $.ajax({
      cache: false,
      url: url,
      data: $(this.form).serialize(),
      type: "POST",
      success: function (response) {
        if (response.redirect)
          location.href = response.redirect;
        else if (response.error)
		logger.debug(`saveEditAddress(url=${url}): location.href → ${location.href}`);
          alert(response.message);

        return true;
      },
      error: function (err) {
        alert(err);
      }
    });
	logger.debug(`<<< Exiting saveEditAddress(url=${url})`);
  },

  deleteEditAddress: function(url, addressId) {
	logger.debug(`>>> Entering deleteEditAddress(url=${url},addressId=${addressId})`);
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
	logger.debug(`<<< Exiting deleteEditAddress(url=${url},addressId=${addressId})`);
	logger.debug(`deleteEditAddress(url=${url},addressId=${addressId}): location.href → ${location.href}`);
  },

  resetBillingForm: function() {
	logger.debug('>>> Entering resetBillingForm()');
    $(':input', '#billingaddress-new-form')
      .not(':button, :submit, :reset, :hidden').removeAttr('checked').removeAttr('selected')
      .not(':checkbox, :radio, select').val('');

    $('select option[value="0"]').prop('selected', true);
	logger.debug('<<< Exiting resetBillingForm()');
  },

  setSelectedStateId: function(id) {
	logger.debug(`>>> Entering setSelectedStateId(id=${id})`);
    this.selectedStateId = id;
	logger.debug(`<<< Exiting setSelectedStateId(id=${id})`);
  }
}