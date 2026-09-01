const logger = console;
var CheckoutShipping = {
  form: false,
  selectedStateId: 0,

  init: function (form) {
	logger.debug(`>>> Entering init(form=${form})`);
    this.form = form;
	logger.debug(`init(form=${form}): form → ${form}`);
  },

  editAddress: function(url, addressId, titleText) {
	logger.debug(`>>> Entering editAddress(url=${url},addressId=${addressId},titleText=${titleText})`);
    CheckoutShipping.resetShippingForm();

	logger.debug(`<<< Exiting editAddress(url=${url},addressId=${addressId},titleText=${titleText})`);
    var prefix = 'ShippingNewAddress_';
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
			logger.debug(`>>> Entering function(id=${id},value=${value})`);
            if (id.indexOf("CustomAddressAttributes") >= 0 && Array.isArray(value)) {
			logger.debug(`<<< Exiting success(data=${data},textStatus=${textStatus},jqXHR=${jqXHR})`);
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
			logger.debug(`<<< Exiting function(id=${id},value=${value})`);
          });
		logger.debug(`>>> Entering success(response=${response})`);
		logger.debug(`<<< Exiting success(response=${response})`);
      },
      complete: function(jqXHR, textStatus) {
		logger.debug(`>>> Entering complete(jqXHR=${jqXHR},textStatus=${textStatus})`);
        $('#shipping-addresses-form').hide();
        $('#shippingaddress-next-button').hide();
        $('#shippingaddress-save-button').show();
        $('#shippingaddress-cancel-button').show();
        $('#shippingaddress-new-form .title-text').html(titleText);
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
      success: function(response) {
        location.href = response.redirect;
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
	logger.debug(`deleteEditAddress(url=${url},addressId=${addressId}): location.href → ${location.href}`);
  },

  resetShippingForm: function() {
	logger.debug('>>> Entering resetShippingForm()');
    $(':input', '#shippingaddress-new-form')
      .not(':button, :submit, :reset, :hidden').removeAttr('checked').removeAttr('selected')
      .not(':checkbox, :radio, select').val('');

    $('select option[value="0"]').prop('selected', true);
	logger.debug('<<< Exiting resetShippingForm()');
  },
}